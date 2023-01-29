using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.API.Services;
using PubNet.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authors/{username}")]
[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
public class AuthorController : ControllerBase
{
    private static ErrorResponse UsernameMismatch =>
        new(new("author-username-mismatch", "The username you are trying to access does not match the owner of the token you used"));

    private readonly ILogger<AuthorController> _logger;
    private readonly PubNetContext _db;
    private readonly PasswordManager _passwordManager;

    public AuthorController(ILogger<AuthorController> logger, PubNetContext db, PasswordManager passwordManager)
    {
        _logger = logger;
        _db = db;
        _passwordManager = passwordManager;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string username, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            var author = await _db.Authors.FirstOrDefaultAsync(a => a.UserName == username, cancellationToken);

            return author is null ? NotFound() : Ok(author);
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Delete([FromQuery] string username, [FromBody] DeleteAuthorRequest dto, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            if (username != author.UserName)
            {
                return Unauthorized(UsernameMismatch);
            }

            await _passwordManager.ThrowForInvalid(_db, author, dto.Password, cancellationToken);

            if (dto.Confirmation != author.UserName)
            {
                return Unauthorized(new ErrorResponse(new("invalid-confirmation", "The confirmation must match your username")));
            }

            _db.Tokens.RemoveRange(author.Tokens);
            _db.Authors.Remove(author);
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new SuccessResponse(new($"Author '{username}' successfully deleted.")));
        }
    }

    [HttpGet("packages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackagesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPackages(string username, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            var author = await _db.Authors
                .Where(a => a.UserName == username)
                .Include(a => a.Packages)
                .FirstOrDefaultAsync(cancellationToken);

            return author is null ? NotFound() : Ok(new PackagesResponse(author.Packages));
        }
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Edit(string username, [FromBody] EditAuthorRequest dto, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.UserName!,
               }))
        {
            if (username != author.UserName)
                return Unauthorized(UsernameMismatch);

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
