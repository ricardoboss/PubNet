using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO;
using PubNet.API.DTO.Errors;

namespace PubNet.API.Controllers;

public abstract class BaseController : ControllerBase
{
	[NonAction]
	protected ObjectResult Error<T>(int status) where T : ErrorMessageDto, new() =>
		StatusCode(status, StatusToDto<T>(status, null, null));

	[NonAction]
	protected ObjectResult Error<T>(int status, string? message) where T : ErrorMessageDto, new() =>
		StatusCode(status, StatusToDto<T>(status, null, message));

	[NonAction]
	protected ObjectResult Error<T>(int status, string? code, string? message) where T : ErrorMessageDto, new() =>
		StatusCode(status, StatusToDto<T>(status, code, message));

	[NonAction]
	private static T StatusToDto<T>(int status, string? errorCode, string? errorMessage) where T : ErrorMessageDto, new()
	{
		errorCode ??= PubNetStatusCodes.ToErrorCode(status);
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
