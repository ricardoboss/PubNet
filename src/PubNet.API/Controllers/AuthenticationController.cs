using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController(
	JwtTokenGenerator tokenGenerator,
	PubNetContext db,
	PasswordManager passwordManager,
	IConfiguration configuration,
	INotificationService notificationService,
	ILogger<AuthenticationController> logger
) : BaseController
{
	private static InvalidCredentialException EmailNotFound => new("E-Mail address not registered");

	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JwtTokenResponse))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Login(LoginRequest dto, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(dto.Email))
			throw EmailNotFound;

		var author = await db.Authors.FirstOrDefaultAsync(a => EF.Functions.ILike(a.Email, dto.Email), cancellationToken);
		if (author is null)
			throw EmailNotFound;

		await passwordManager.ThrowForInvalid(db, author, dto.Password, cancellationToken);

		var token = tokenGenerator.Generate(author, out var expiresAt);
		return Ok(new JwtTokenResponse(token, expiresAt));
	}

	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
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

		if (db.Authors.Any(a => EF.Functions.ILike(a.UserName, dto.Username)))
			return UnprocessableEntity(ErrorResponse.UsernameAlreadyInUse);

		if (db.Authors.Any(a => EF.Functions.ILike(a.Email, dto.Email)))
			return UnprocessableEntity(ErrorResponse.EmailAlreadyInUse);

		var author = new Author
		{
			UserName = dto.Username,
			Email = dto.Email,
			Name = dto.Name,
			Website = dto.Website,
			Inactive = false,
			RegisteredAtUtc = DateTimeOffset.UtcNow,
			Packages = new List<Package>(),
		};

		author.PasswordHash = await passwordManager.GenerateHashAsync(author, dto.Password, cancellationToken);

		db.Authors.Add(author);
		await db.SaveChangesAsync(cancellationToken);

		try
		{
			var referer = Request.Headers.Referer.FirstOrDefault() ?? Request.Host.ToString();
			await notificationService.SendWelcomeNotificationAsync(author, new(referer), cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to send welcome notification");
		}

		return CreatedAtAction("Get", "Authors", new { username = author.UserName }, author);
	}

	[HttpGet("self")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Self(ApplicationRequestContext context,
		CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		return Ok(AuthorDto.FromAuthor(author, true));
	}

	[HttpGet("registrations-enabled")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
	public bool RegistrationsEnabled()
	{
		return configuration.GetValue<bool?>("OpenRegistration") ?? false;
	}
}
