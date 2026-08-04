using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authentication.Errors;
using PubNet.API.DTO.Authors;
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
	IAuthorRegistrationService registrationService,
	IOnboardingService onboardingService,
	INotificationService notificationService,
	IPasswordResetService passwordResetService,
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
			return Error<EmailNotFoundErrorDto>(PubNetStatusCodes.Status460EmailNotFound);

		var author =
			await db.Authors.FirstOrDefaultAsync(a => EF.Functions.ILike(a.Email, dto.Email), cancellationToken);
		if (author is null)
			return Error<EmailNotFoundErrorDto>(PubNetStatusCodes.Status460EmailNotFound);

		if (!await passwordManager.IsValid(db, author, dto.Password, cancellationToken))
			return Error<InvalidPasswordErrorDto>(PubNetStatusCodes.Status461InvalidPassword);

		var token = tokenGenerator.Generate(author, out var expiresAt);
		return Ok(new JsonWebTokenResponseDto(token, expiresAt));
	}

	[AllowAnonymous]
	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status400BadRequest, Type = typeof(MissingRegistrationDataErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status462RegistrationsDisabled, Type = typeof(RegistrationsDisabledErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status463UsernameAlreadyInUse, Type = typeof(UsernameAlreadyInUseErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status464EmailAlreadyInUse, Type = typeof(EmailAlreadyInUseErrorDto))]
	public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto,
		CancellationToken cancellationToken = default)
	{
		if (!await RegistrationsEnabledAsync(cancellationToken))
			return Error<RegistrationsDisabledErrorDto>(PubNetStatusCodes.Status462RegistrationsDisabled);

		// the incomplete-request and duplicate checks live in the registration service, so that registration
		// and the first-time setup reject the same things in the same way
		Author author;
		try
		{
			author = await registrationService.RegisterAsync(dto, Role.Default, cancellationToken);
		}
		catch (AuthorRegistrationException e)
		{
			return RegistrationError(e);
		}

		try
		{
			await notificationService.SendWelcomeNotificationAsync(author, RefererUri, cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to send welcome notification");
		}

		// MIND: must not return the entity itself - Author derives from IdentityUser and would serialize
		// PasswordHash, SecurityStamp and friends
		return CreatedAtAction("Get", "Authors", new { username = author.UserName },
			AuthorDto.FromAuthor(author, true));
	}

	[AllowAnonymous]
	[HttpPost("forgot-password")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto dto,
		CancellationToken cancellationToken = default)
	{
		// always answers 204: whether an account exists for an address is none of the caller's business
		if (string.IsNullOrWhiteSpace(dto.Email))
			return NoContent();

		var author =
			await db.Authors.FirstOrDefaultAsync(a => EF.Functions.ILike(a.Email, dto.Email), cancellationToken);
		if (author is null)
			return NoContent();

		var token = await passwordResetService.GenerateTokenAsync(author, cancellationToken);

		try
		{
			await notificationService.SendPasswordResetNotificationAsync(author, token, RefererUri, cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to send password reset notification");
		}

		return NoContent();
	}

	[AllowAnonymous]
	[HttpPost("reset-password")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(PubNetStatusCodes.Status461InvalidPassword, Type = typeof(InvalidPasswordErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status466InvalidPasswordResetToken, Type = typeof(InvalidPasswordResetTokenErrorDto))]
	public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto dto,
		CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(dto.Token))
			return Error<InvalidPasswordResetTokenErrorDto>(PubNetStatusCodes.Status466InvalidPasswordResetToken);

		if (string.IsNullOrWhiteSpace(dto.Password))
			return Error<InvalidPasswordErrorDto>(PubNetStatusCodes.Status461InvalidPassword,
				"The new password must not be empty");

		try
		{
			await passwordResetService.ResetPasswordAsync(dto.Token, dto.Password, cancellationToken);
		}
		catch (InvalidPasswordResetTokenException)
		{
			return Error<InvalidPasswordResetTokenErrorDto>(PubNetStatusCodes.Status466InvalidPasswordResetToken);
		}

		return NoContent();
	}

	[Authorize]
	[HttpGet("self")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
	public async Task<IActionResult> Self(ApplicationRequestContext context,
		CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		return Ok(AuthorDto.FromAuthor(author, true, true));
	}

	[AllowAnonymous]
	[HttpGet("registrations-enabled")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
	public async Task<bool> RegistrationsEnabledAsync(CancellationToken cancellationToken = default)
	{
		// until the instance has been set up, the only way to create an account is onboarding
		if (await onboardingService.IsPendingAsync(cancellationToken))
			return false;

		return configuration.GetValue<bool?>("OpenRegistration") ?? false;
	}
}
