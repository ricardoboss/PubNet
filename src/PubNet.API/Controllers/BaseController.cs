using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO;
using PubNet.API.DTO.Errors;

namespace PubNet.API.Controllers;

public abstract class BaseController : ControllerBase
{
	[NonAction]
	protected ObjectResult Error<T>(int status, string? message = null) where T : ErrorMessageDto, new() =>
		StatusCode(status, StatusToDto<T>(status, message));

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
