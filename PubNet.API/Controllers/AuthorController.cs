using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.API.Models;
using PubNet.API.Services;

namespace PubNet.API.Controllers;

[ApiController]
[Route("author")]
public class AuthorController : ControllerBase
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

    [HttpGet("{email}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string email)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorEmail"] = email
               }))
        {
            var author = await GetAuthorFromEmail(email);

            return author is null ? NotFound() : Ok(author);
        }
    }

    [HttpGet("{email}/packages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Package>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPackages(string email)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorEmail"] = email
               }))
        {
            var author = await GetAuthorFromEmail(email);

            return author is null ? NotFound() : Ok(author.Packages);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
    {
        var author = await _db.Authors.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (author is not null)
        {
            return UnprocessableEntity(new ErrorResponse(new("email-already-in-use",
                "The e-mail address you provided is already in use.")));
        }

        author = new()
        {
            Email = dto.Email,
            Name = dto.Name,
            Website = dto.Website,
            Inactive = false,
            RegisteredAtUtc = DateTimeOffset.UtcNow,
            Packages = new(),
            Tokens = new(),
        };

        _db.Authors.Add(author);

        author.PasswordHash = _passwordHasher.HashPassword(author, dto.Password);

        await _db.SaveChangesAsync();

        return CreatedAtAction("Get", new { email = author.Email }, author);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Edit([FromBody] EditAuthorRequest dto, [FromServices] ApplicationRequestContext context)
    {
        var author = context.Author;
        if (author is null)
        {
            throw new BearerTokenException("Missing token or token invalid. Get a new token at " + CreatedAtAction("Get", null));
        }

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorEmail"] = author.Email
               }))
        {
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

    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorToken))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Token([FromBody] CreateTokenRequest dto)
    {
        var author = await RequireAuthorFromEmail(dto.Email);

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorEmail"] = author.Email
               }))
        {
            await VerifyPassword(author, dto.Password);

            var token = await _tokenDispenser.Dispense(dto.Name, author, TimeSpan.FromDays(30));

            return Ok(token);
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

            throw new UnauthorizedAccessException();
        }
    }

    private async Task<Author?> GetAuthorFromEmail(string email)
    {
        return await _db.Authors.FirstOrDefaultAsync(a => a.Email == email);
    }

    private async Task<Author> RequireAuthorFromEmail(string email)
    {
        var author = await GetAuthorFromEmail(email);
        if (author is null)
        {
            throw new UnauthorizedAccessException();
        }

        return author;
    }

    private async Task<Author> RequireAuthorFromToken(string token)
    {
        var authorToken = await _db.Tokens
            .Where(t => t.Value == token)
            .Include(t => t.Owner)
            .FirstOrDefaultAsync();
        if (authorToken is null || authorToken.Owner is null)
        {
            throw new BearerTokenException("Token or owner not found");
        }

        if (!authorToken.IsValid())
        {
            throw new BearerTokenException("Token expired");
        }

        return authorToken.Owner;
    }
}