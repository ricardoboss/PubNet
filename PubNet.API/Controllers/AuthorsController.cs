using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.API.Services;
using PubNet.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authors")]
[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
public class AuthorsController : BaseController
{
    private readonly ILogger<AuthorsController> _logger;
    private readonly PubNetContext _db;
    private readonly PasswordManager _passwordManager;

    public AuthorsController(ILogger<AuthorsController> logger, PubNetContext db, PasswordManager passwordManager)
    {
        _logger = logger;
        _db = db;
        _passwordManager = passwordManager;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ResponseCache(VaryByQueryKeys = new[] { "q", "before", "limit" }, Location = ResponseCacheLocation.Any, Duration = 3600)]
    public IActionResult GetAll([FromQuery] string? q = null, [FromQuery] long? before = null, [FromQuery] int? limit = null)
    {
        const int maxLimit = 1000;

        IQueryable<Author> packages = _db.Authors
                .Where(a => !a.Inactive)
                .OrderByDescending(p => p.RegisteredAtUtc)
            ;

        if (q != null)
        {
            packages = packages.Where(a => a.UserName!.StartsWith(q));
        }

        if (before.HasValue)
        {
            if (!limit.HasValue)
            {
                return BadRequest(ErrorResponse.InvalidQuery);
            }

            var publishedAtUpperLimit = DateTimeOffset.FromUnixTimeMilliseconds(before.Value);

            packages = packages.Where(p => p.RegisteredAtUtc < publishedAtUpperLimit);
        }

        if (limit.HasValue)
        {
            var resultLimit = Math.Min(limit.Value, maxLimit);

            packages = packages.Take(resultLimit);
        }

        return Ok(new AuthorsResponse(packages.Select(a => new SearchResultAuthor(a.UserName, a.Name, a.Packages.Count))));
    }

    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string username, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["AuthorUsername"] = username,
        }))
        {
            var author = await _db.Authors.FirstOrDefaultAsync(a => a.UserName == username, cancellationToken);

            return author is null ? NotFound() : Ok(AuthorDto.FromAuthor(author));
        }
    }

    [HttpPost("{username}/delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Delete([FromRoute] string username, [FromBody] DeleteAuthorRequest dto, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["AuthorUsername"] = username,
        }))
        {
            if (username != author.UserName) return Unauthorized(ErrorResponse.UsernameMismatch);

            if (!await _passwordManager.IsValid(_db, author, dto.Password, cancellationToken))
                return Unauthorized(ErrorResponse.InvalidPasswordConfirmation);

            foreach (var authorPackage in _db.Packages.Where(p => p.Author == author))
                authorPackage.Author = null;

            await _db.SaveChangesAsync(cancellationToken);

            _db.Authors.Remove(author);

            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new SuccessResponse(new($"Author '{username}' successfully deleted.")));
        }
    }

    [HttpGet("{username}/packages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorPackagesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPackages(string username, CancellationToken cancellationToken = default)
    {
        var author = await _db.Authors
            .Where(a => a.UserName == username)
            .Include(a => a.Packages)
            .FirstOrDefaultAsync(cancellationToken);

        return author is null ? NotFound() : Ok(new AuthorPackagesResponse(author.Packages.Select(PackageDto.FromPackage)));
    }

    [HttpPatch("{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Edit(string username, [FromBody] EditAuthorRequest dto, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["AuthorUsername"] = author.UserName,
        }))
        {
            if (username != author.UserName) return Unauthorized(ErrorResponse.UsernameMismatch);

            if (dto.Name is not null && author.Name != dto.Name)
            {
                author.Name = dto.Name;
            }

            if (string.IsNullOrWhiteSpace(dto.Website) && author.Website is not null)
            {
                author.Website = null;
            }
            else if (author.Website != dto.Website)
            {
                author.Website = dto.Website;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
