using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Errors;
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
	[AllowAnonymous]
	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JsonWebTokenResponseDto))]
	[ProducesResponseType(PubNetStatusCodes.Status460EmailNotFound, Type = typeof(EmailNotFoundErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status461InvalidPassword, Type = typeof(InvalidPasswordErrorDto))]
	public async Task<IActionResult> Login(LoginRequestDto dto, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(dto.Email))
			return Error(PubNetStatusCodes.Status460EmailNotFound);

		var author =
			await db.Authors.FirstOrDefaultAsync(a => EF.Functions.ILike(a.Email, dto.Email), cancellationToken);
		if (author is null)
			return Error(PubNetStatusCodes.Status460EmailNotFound);

		if (!await passwordManager.IsValid(db, author, dto.Password, cancellationToken))
			return Error(PubNetStatusCodes.Status461InvalidPassword);

		var token = tokenGenerator.Generate(author, out var expiresAt);
		return Ok(new JsonWebTokenResponseDto(token, expiresAt));
	}

	[AllowAnonymous]
	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorDto))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponseDto))]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponseDto))]
	public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto,
		CancellationToken cancellationToken = default)
	{
		if (!RegistrationsEnabled())
			return BadRequest(ErrorResponseDto.RegistrationsDisabled);

		if (dto.Username is null || dto.Name is null || dto.Password is null || dto.Email is null)
			return UnprocessableEntity(ErrorResponseDto.MissingValues);

		if (db.Authors.Any(a => EF.Functions.ILike(a.UserName, dto.Username)))
			return UnprocessableEntity(ErrorResponseDto.UsernameAlreadyInUse);

		if (db.Authors.Any(a => EF.Functions.ILike(a.Email, dto.Email)))
			return UnprocessableEntity(ErrorResponseDto.EmailAlreadyInUse);

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

	[Authorize]
	[HttpGet("self")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponseDto))]
	public async Task<IActionResult> Self(ApplicationRequestContext context,
		CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		return Ok(AuthorDto.FromAuthor(author, true));
	}

	[AllowAnonymous]
	[HttpGet("registrations-enabled")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
	public bool RegistrationsEnabled()
	{
		return configuration.GetValue<bool?>("OpenRegistration") ?? false;
	}
}
