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
    private readonly BearerTokenManager _bearerTokenManager;

    public AuthorController(ILogger<AuthorController> logger, PubNetContext db, IPasswordHasher<Author> passwordHasher,
        AuthorTokenDispenser tokenDispenser, BearerTokenManager bearerTokenManager)
    {
        _logger = logger;
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenDispenser = tokenDispenser;
        _bearerTokenManager = bearerTokenManager;
    }

    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string username)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            var author = await GetAuthorFromUsername(username);

            return author is null ? NotFound() : Ok(author);
        }
    }

    [HttpDelete("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromQuery] string username, [FromBody] DeleteAuthorRequest dto, [FromServices] ApplicationRequestContext context)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            if (username != author.Username)
            {
                return Unauthorized(UsernameMismatch);
            }

            await VerifyPassword(author, dto.Password);

            if (dto.Confirmation != author.Username)
            {
                return Unauthorized(new ErrorResponse(new("invalid-confirmation", "The confirmation must match your username")));
            }

            _db.Tokens.RemoveRange(author.Tokens);
            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();

            return Ok(new SuccessResponse(new($"Author '{username}' successfully deleted.")));
        }
    }

    [HttpGet("{username}/packages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackagesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPackages(string username)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            var author = await GetAuthorFromUsername(username);

            return author is null ? NotFound() : Ok(new PackagesResponse(author.Packages));
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
    {
        if (_db.Authors.Any(a => a.Username == dto.Username))
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
            Username = dto.Username,
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
        await _db.SaveChangesAsync();

        return CreatedAtAction("Get", new { username = author.Username }, author);
    }

    [HttpPatch("{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Edit(string username, [FromBody] EditAuthorRequest dto, [FromServices] ApplicationRequestContext context)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.Username,
               }))
        {
            if (username != author.Username)
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

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    [HttpGet("{username}/tokens")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BearerTokenResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public IActionResult GetTokens(string username, ApplicationRequestContext context)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.Username,
               }))
        {
            if (username != author.Username)
            {
                return Unauthorized(UsernameMismatch);
            }

            var tokens = author.Tokens
                .Select(t => new BearerTokenResponse(t.Name, _bearerTokenManager.Generate(t), t.ExpiresAtUtc));

            return Ok(tokens);
        }
    }

    [HttpPost("{username}/tokens")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BearerTokenResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateToken(string username, [FromBody] CreateTokenRequest dto, [FromServices] ApplicationRequestContext context)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = username,
               }))
        {
            var author = await GetAuthorFromUsername(username);
            if (author is null)
            {
                return NotFound();
            }

            await VerifyPassword(author, dto.Password);

            if (!TokenNameRegex().IsMatch(dto.TokenName))
            {
                return UnprocessableEntity(new ErrorResponse(new("invalid-token-name", "The token name must consist of at least one lowercase, alphanumeric, underscore or hyphen character.")));
            }

            if (author.Tokens.Any(t => t.Name == dto.TokenName))
            {
                return UnprocessableEntity(new ErrorResponse(new("duplicate-token-name", $"The name '{dto.TokenName}' is already in use. Delete it or choose a different name")));
            }

            var authorToken = await _tokenDispenser.Dispense(dto.TokenName, author, TimeSpan.FromDays(30));
            var bearer = _bearerTokenManager.Generate(authorToken);

            return StatusCode(StatusCodes.Status201Created, new BearerTokenResponse(authorToken.Name, bearer, authorToken.ExpiresAtUtc));
        }
    }

    [HttpDelete("{username}/tokens/{tokenName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToken(string username, string tokenName, ApplicationRequestContext context)
    {
        var author = context.RequireAuthor();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.Username,
               }))
        {
            if (username != author.Username)
            {
                return Unauthorized(UsernameMismatch);
            }

            var token = author.Tokens.FirstOrDefault(t => t.Name == tokenName);
            if (token is null)
            {
                return NotFound();
            }

            _db.Tokens.Remove(token);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    private async Task VerifyPassword(Author author, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(author, author.PasswordHash, password);
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            author.PasswordHash = _passwordHasher.HashPassword(author, password);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Rehashed password for {@Author}", author);
        }
        else if (result != PasswordVerificationResult.Success)
        {
            _logger.LogInformation("Wrong password for {@Author}", author);

            throw new InvalidCredentialException("Password verification failed");
        }
    }

    private async Task<Author?> GetAuthorFromUsername(string username) =>
        await _db.Authors.Where(a => a.Username == username)
            .Include(nameof(Author.Packages))
            .Include(nameof(Author.Packages) + "." + nameof(Package.Versions))
            .FirstOrDefaultAsync();

    private static ErrorResponse UsernameMismatch =>
        new(new("author-username-mismatch", "The username you are trying to access does not match the owner of the token you used"));

    [GeneratedRegex("^[a-z0-9-_]+$")]
    private static partial Regex TokenNameRegex();
}