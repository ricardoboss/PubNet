using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO;

namespace PubNet.API.Controllers;

public abstract class BaseController : ControllerBase
{
	[NonAction]
	protected ObjectResult Error<T>(int status) where T : ErrorMessageDto, IHaveDefaultMessage, new() =>
		StatusCode(status, StatusToDto<T>(status, null));

	[NonAction]
	protected ObjectResult Error<T>(int status, string message) where T : ErrorMessageDto, new() =>
		StatusCode(status, StatusToDto<T>(status, message));

	[NonAction]
	private static T StatusToDto<T>(int status, string? errorMessage) where T : ErrorMessageDto, new()
	{
		var errorCode = PubNetStatusCodes.ToErrorCode(status);
		if (errorCode is null)
			throw new NotImplementedException("No error code defined for status code: " + status);

		T wrapper = new();

		errorMessage ??= wrapper is IHaveDefaultMessage d
			? d.DefaultMessage
			: throw new NotImplementedException("No default error message defined for status code: " + status);

		wrapper.Error = new()
		{
			Code = errorCode,
			Message = errorMessage,
		};

		return wrapper;
	}
}
