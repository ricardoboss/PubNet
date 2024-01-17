using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.Services;
using PubNet.Database.Context;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;
using PubNet.Database.Entities.Nuget;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController : BaseController
{
	private readonly PubNetContext _db;
	private readonly PasswordManager _passwordManager;
	private readonly JwtTokenGenerator _tokenGenerator;
	private readonly IConfiguration _configuration;

	public AuthenticationController(JwtTokenGenerator tokenGenerator, PubNetContext db, PasswordManager passwordManager, IConfiguration configuration)
	{
		_tokenGenerator = tokenGenerator;
		_db = db;
		_passwordManager = passwordManager;
		_configuration = configuration;
	}

	private static InvalidCredentialException EmailNotFound => new("E-Mail address not registered");

	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JwtTokenResponse))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Login(LoginRequest dto, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(dto.Email))
			throw EmailNotFound;

		var identity = await _db.Identities.FirstOrDefaultAsync(i => EF.Functions.ILike(i.Email, dto.Email), cancellationToken);
		if (identity is null)
			throw EmailNotFound;

		await _passwordManager.ThrowForInvalid(_db, identity, dto.Password, cancellationToken);

		var token = _tokenGenerator.Generate(identity, out var expiresAt);
		return Ok(new JwtTokenResponse(token, expiresAt));
	}

	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorDto))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Register([FromBody] RegisterRequest dto,
		CancellationToken cancellationToken = default)
	{
		if (!RegistrationsEnabled())
			return BadRequest(ErrorResponse.RegistrationsDisabled);

		if (dto.Username is null || dto.Name is null || dto.Password is null || dto.Email is null)
			return UnprocessableEntity(ErrorResponse.MissingValues);

		if (_db.Authors.Any(a => EF.Functions.ILike(a.UserName, dto.Username)))
			return UnprocessableEntity(ErrorResponse.UsernameAlreadyInUse);

		if (_db.Authors.Any(a => EF.Functions.ILike(a.Identity.Email, dto.Email)))
			return UnprocessableEntity(ErrorResponse.EmailAlreadyInUse);

		var author = new Author
		{
			UserName = dto.Username,
			RegisteredAt = DateTimeOffset.UtcNow,
			DartPackages = new List<DartPackage>(),
			NugetPackages = new List<NugetPackage>(),
		};

		_db.Authors.Add(author);

		var identity = new Identity
		{
			Email = dto.Email,
			Author = author,
		};

		identity.PasswordHash = await _passwordManager.GenerateHashAsync(identity, dto.Password, cancellationToken);

		_db.Identities.Add(identity);

		await _db.SaveChangesAsync(cancellationToken);

		return CreatedAtAction("Get", "Authors", new { username = author.UserName }, author);
	}

	[HttpGet("self")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Self(ApplicationRequestContext context,
		CancellationToken cancellationToken = default)
	{
		var identity = await context.RequireIdentityAsync(User, _db, cancellationToken);

		return Ok(AuthorDto.FromAuthor(identity.Author, true));
	}

	[HttpGet("registrations-enabled")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
	public bool RegistrationsEnabled()
	{
		return _configuration.GetValue<bool?>("OpenRegistration") ?? false;
	}
}
