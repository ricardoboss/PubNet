using System.Security.Authentication;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.Models;
using PubNet.API.Services;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authors")]
public partial class AuthorController : ControllerBase
{
    private readonly ILogger<AuthorController> _logger;
    private readonly PubNetContext _db;
    private readonly IPasswordHasher<Author> _passwordHasher;
    private readonly AuthorTokenDispenser _tokenDispenser;

    public AuthorController(ILogger<AuthorController> logger, PubNetContext db, IPasswordHasher<Author> passwordHasher,
        AuthorTokenDispenser tokenDispenser)
    {
        _logger = logger;
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenDispenser = tokenDispenser;
    }

    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
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

    [HttpDelete("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromQuery] string username, [FromBody] DeleteAuthorRequest dto, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            if (username != author.UserName)
            {
                return Unauthorized(UsernameMismatch);
            }

            await VerifyPassword(author, dto.Password, cancellationToken);

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

    [HttpGet("{username}/packages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackagesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
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

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest dto, CancellationToken cancellationToken = default)
    {
        if (_db.Authors.Any(a => a.UserName == dto.Username))
        {
            return UnprocessableEntity(new ErrorResponse(new("username-already-in-use",
                "The username you provided is already in use.")));
        }

        if (_db.Authors.Any(a => a.Email == dto.Email))
        {
            return UnprocessableEntity(new ErrorResponse(new("email-already-in-use",
                "The e-mail address you provided is already in use.")));
        }

        var author = new Author
        {
            UserName = dto.Username,
            Email = dto.Email,
            Name = dto.Name,
            Website = dto.Website,
            Inactive = false,
            RegisteredAtUtc = DateTimeOffset.UtcNow,
            Packages = new List<Package>(),
            Tokens = new List<AuthorToken>(),
        };

        author.PasswordHash = _passwordHasher.HashPassword(author, dto.Password);

        _db.Authors.Add(author);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("Get", new { username = author.UserName }, author);
    }

    [HttpPatch("{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Edit(string username, [FromBody] EditAuthorRequest dto, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.UserName,
               }))
        {
            if (username != author.UserName)
            {
                return Unauthorized(UsernameMismatch);
            }

            if (dto.Name is not null && author.Name != dto.Name)
            {
                author.Name = dto.Name;
            }

            if (dto.RemoveWebsite && author.Website is not null)
            {
                author.Website = null;
            }
            else if (dto.Website is not null && author.Website != dto.Website)
            {
                author.Website = dto.Website;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }

    [HttpGet("{username}/tokens")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BearerTokenResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public IActionResult GetTokens(string username, [FromServices] ApplicationRequestContext context, [FromServices] BearerTokenManager bearerTokenManager)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.UserName!,
               }))
        {
            if (username != author.UserName)
            {
                return Unauthorized(UsernameMismatch);
            }

            var tokens = author.Tokens
                .Select(t => new BearerTokenResponse(t.Name, bearerTokenManager.Generate(t), t.ExpiresAtUtc));

            return Ok(tokens);
        }
    }

    [HttpPost("{username}/tokens")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BearerTokenResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateToken(string username, [FromBody] CreateTokenRequest dto, [FromServices] BearerTokenManager bearerTokenManager, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            var author = await _db.Authors.FirstOrDefaultAsync(a => a.UserName == username, cancellationToken);
            if (author is null)
            {
                return NotFound();
            }

            await VerifyPassword(author, dto.Password, cancellationToken);

            if (!TokenNameRegex().IsMatch(dto.TokenName))
            {
                return UnprocessableEntity(new ErrorResponse(new("invalid-token-name", "The token name must consist of at least one lowercase, alphanumeric, underscore or hyphen character.")));
            }

            if (author.Tokens.Any(t => t.Name == dto.TokenName))
            {
                return UnprocessableEntity(new ErrorResponse(new("duplicate-token-name", $"The name '{dto.TokenName}' is already in use. Delete it or choose a different name")));
            }

            var authorToken = await _tokenDispenser.Dispense(dto.TokenName, author, TimeSpan.FromDays(30));
            var bearer = bearerTokenManager.Generate(authorToken);

            return StatusCode(StatusCodes.Status201Created, new BearerTokenResponse(authorToken.Name, bearer, authorToken.ExpiresAtUtc));
        }
    }

    [HttpDelete("{username}/tokens/{tokenName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToken(string username, string tokenName, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.UserName,
               }))
        {
            if (username != author.UserName)
            {
                return Unauthorized(UsernameMismatch);
            }

            var token = author.Tokens.FirstOrDefault(t => t.Name == tokenName);
            if (token is null)
            {
                return NotFound();
            }

            _db.Tokens.Remove(token);
            await _db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }

    private async Task VerifyPassword(Author author, string password, CancellationToken cancellationToken = default)
    {
        var result = _passwordHasher.VerifyHashedPassword(author, author.PasswordHash, password);
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            author.PasswordHash = _passwordHasher.HashPassword(author, password);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Rehashed password for {@Author}", author);
        }
        else if (result != PasswordVerificationResult.Success)
        {
            _logger.LogInformation("Wrong password for {@Author}", author);

            throw new InvalidCredentialException("Password verification failed");
        }
    }

    private static ErrorResponse UsernameMismatch =>
        new(new("author-username-mismatch", "The username you are trying to access does not match the owner of the token you used"));

    [GeneratedRegex("^[a-z0-9-_]+$")]
    private static partial Regex TokenNameRegex();
}