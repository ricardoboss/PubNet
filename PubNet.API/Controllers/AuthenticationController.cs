using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.API.Services;
using PubNet.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    private static InvalidCredentialException EmailNotFound => new("E-Mail address not registered");
    private static ErrorResponse UsernameAlreadyInUse => new(new("username-already-in-use",
        "The username you provided is already in use."));
    private static ErrorResponse EmailAlreadyInUse => new(new("email-already-in-use",
        "The e-mail address you provided is already in use."));

    private readonly ILogger<AuthenticationController> _logger;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly PubNetContext _db;
    private readonly PasswordManager _passwordManager;

    public AuthenticationController(ILogger<AuthenticationController> logger, JwtTokenGenerator tokenGenerator, PubNetContext db, PasswordManager passwordManager)
    {
        _logger = logger;
        _tokenGenerator = tokenGenerator;
        _db = db;
        _passwordManager = passwordManager;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JwtTokenResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Login(LoginRequest dto, CancellationToken cancellationToken = default)
    {
        var author = await _db.Authors.FirstOrDefaultAsync(a => a.Email == dto.Email, cancellationToken);
        if (author is null)
            throw EmailNotFound;

        await _passwordManager.ThrowForInvalid(_db, author, dto.Password, cancellationToken);

        var token = _tokenGenerator.Generate(author, out var expiresAt);
        return Ok(new JwtTokenResponse(token, expiresAt));
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest dto, CancellationToken cancellationToken = default)
    {
        if (_db.Authors.Any(a => a.UserName == dto.Username))
            return UnprocessableEntity(UsernameAlreadyInUse);

        if (_db.Authors.Any(a => a.Email == dto.Email))
            return UnprocessableEntity(EmailAlreadyInUse);

        var author = new Author
        {
            UserName = dto.Username,
            Email = dto.Email,
            Name = dto.Name!,
            Website = dto.Website,
            Inactive = false,
            RegisteredAtUtc = DateTimeOffset.UtcNow,
            Packages = new List<Package>(),
            Tokens = new List<AuthorToken>(),
        };

        author.PasswordHash = await _passwordManager.GenerateHashAsync(author, dto.Password!, cancellationToken);

        _db.Authors.Add(author);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("Get", "Author", new { username = author.UserName }, author);
    }

    [HttpGet("self")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Self(ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        return Ok(await context.RequireAuthorAsync(User, _db, cancellationToken));
    }
}
