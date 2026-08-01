using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.API.Services;

namespace PubNet.API.Controllers;

/// <summary>
/// First-time setup of an instance. Usable without the frontend:
/// <c>curl -X POST .../api/onboarding/complete -H 'Content-Type: application/json' -d '{...}'</c>.
/// </summary>
[ApiController]
[Route("onboarding")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class OnboardingController(
	IOnboardingService onboardingService,
	INotificationService notificationService,
	ILogger<OnboardingController> logger
) : BaseController
{
	[HttpGet("status")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OnboardingStatusDto))]
	public async Task<OnboardingStatusDto> StatusAsync(CancellationToken cancellationToken = default)
	{
		return new()
		{
			Pending = await onboardingService.IsPendingAsync(cancellationToken),
		};
	}

	[HttpPost("complete")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorDto))]
	[ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> CompleteAsync([FromBody] RegisterRequest dto,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var author = await onboardingService.CompleteAsync(dto, cancellationToken);

			logger.LogInformation(
				"Onboarding completed from {RemoteIpAddress}: {Username} (author {AuthorId}) is now an admin",
				HttpContext.Connection.RemoteIpAddress, author.UserName, author.Id);

			try
			{
				await notificationService.SendSetupCompletedNotificationAsync(author, RefererUri, cancellationToken);
			}
			catch (Exception e)
			{
				// the instance is set up either way, so a broken mail configuration must not fail the request
				logger.LogError(e, "Failed to send the setup completed notification");
			}

			return CreatedAtAction("Get", "Authors", new { username = author.UserName },
				AuthorDto.FromAuthor(author, true));
		}
		catch (OnboardingNotPendingException)
		{
			return Rejected("onboarding-not-pending", Conflict(ErrorResponse.OnboardingNotPending));
		}
		catch (AuthorRegistrationException e)
		{
			return Rejected(e.Code, UnprocessableEntity(e.Response));
		}

		IActionResult Rejected(string reason, IActionResult result)
		{
			// whoever completes onboarding becomes admin, so every failed attempt is worth seeing
			logger.LogWarning(
				"Rejected an onboarding attempt from {RemoteIpAddress} for {Username}: {Reason}",
				HttpContext.Connection.RemoteIpAddress, dto.Username, reason);

			return result;
		}
	}
}
