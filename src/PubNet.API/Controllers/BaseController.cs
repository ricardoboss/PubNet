using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Errors;

namespace PubNet.API.Controllers;

public abstract class BaseController : ControllerBase
{
	[NonAction]
	protected ObjectResult FailedDependency(object? result)
	{
		return StatusCode(StatusCodes.Status424FailedDependency, result);
	}

	[NonAction]
	protected ObjectResult Error(int code, string? message = null) =>
		StatusCode(code, StatusToDto(code, message));

	[NonAction]
	private static GenericErrorDto StatusToDto(int status, string? message)
	{
		var errorCode = PubNetStatusCodes.ToErrorCode(status);
		if (errorCode is null)
			throw new NotImplementedException("No error code defined for status code: " + status);

		message ??= PubNetStatusCodes.ToErrorMessage(status);
		if (message is null)
			throw new NotImplementedException("No default error message defined for status code: " + status);

		return new()
		{
			Error = new()
			{
				Code = errorCode,
				Message = message,
			},
		};
	}
}
