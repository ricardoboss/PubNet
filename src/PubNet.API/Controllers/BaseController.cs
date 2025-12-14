using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO;
using PubNet.API.DTO.Authentication.Errors;
using PubNet.API.DTO.Errors;
using PubNet.API.Services;

namespace PubNet.API.Controllers;

public abstract class BaseController : ControllerBase
{
	/// <summary>
	/// Where the caller came from, used to point mails back at the frontend they used.
	/// </summary>
	protected Uri RefererUri => new(Request.Headers.Referer.FirstOrDefault() ?? Request.Host.ToString());

	[NonAction]
	protected ObjectResult Error<T>(int status, string? message = null) where T : ErrorMessageDto, new() =>
		StatusCode(status, StatusToDto<T>(status, message));

	/// <summary>
	/// Translates a rejected registration into the granular DTO for its cause, so that registration and the
	/// first-time setup report it identically.
	/// </summary>
	[NonAction]
	protected ObjectResult RegistrationError(AuthorRegistrationException e) => e.StatusCode switch
	{
		PubNetStatusCodes.Status463UsernameAlreadyInUse =>
			Error<UsernameAlreadyInUseErrorDto>(e.StatusCode, e.Message),
		PubNetStatusCodes.Status464EmailAlreadyInUse =>
			Error<EmailAlreadyInUseErrorDto>(e.StatusCode, e.Message),
		_ => Error<MissingRegistrationDataErrorDto>(PubNetStatusCodes.Status400BadRequest, e.Message),
	};

	[NonAction]
	private static T StatusToDto<T>(int status, string? errorMessage) where T : ErrorMessageDto, new()
	{
		var errorCode = PubNetStatusCodes.ToErrorCode(status);
		if (errorCode is null)
			throw new NotImplementedException("No error code defined for status code: " + status);

		errorMessage ??= PubNetStatusCodes.ToErrorMessage(status);
		if (errorMessage is null)
			throw new NotImplementedException("No default error message defined for status code: " + status);

		return new()
		{
			Error = new()
			{
				Code = errorCode,
				Message = errorMessage,
			},
		};
	}
}
