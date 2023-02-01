using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.Services;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController : BaseController
{
	private readonly PubNetContext _db;
	private readonly PasswordManager _passwordManager;

	private readonly JwtTokenGenerator _tokenGenerator;

	public AuthenticationController(JwtTokenGenerator tokenGenerator, PubNetContext db, PasswordManager passwordManager)
	{
		_tokenGenerator = tokenGenerator;
		_db = db;
		_passwordManager = passwordManager;
	}

	private static InvalidCredentialException EmailNotFound => new("E-Mail address not registered");

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
			return UnprocessableEntity(ErrorResponse.UsernameAlreadyInUse);

		if (_db.Authors.Any(a => a.Email == dto.Email))
			return UnprocessableEntity(ErrorResponse.EmailAlreadyInUse);

		var author = new Author
		{
			UserName = dto.Username!,
			Email = dto.Email,
			Name = dto.Name!,
			Website = dto.Website,
			Inactive = false,
			RegisteredAtUtc = DateTimeOffset.UtcNow,
			Packages = new List<Package>(),
		};

		author.PasswordHash = await _passwordManager.GenerateHashAsync(author, dto.Password!, cancellationToken);

		_db.Authors.Add(author);
		await _db.SaveChangesAsync(cancellationToken);

		return CreatedAtAction("Get", "Authors", new { username = author.UserName }, author);
	}

	[HttpGet("self")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Self(ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		return Ok(await context.RequireAuthorAsync(User, _db, cancellationToken));
	}
}
