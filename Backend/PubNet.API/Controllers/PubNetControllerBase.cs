using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Errors;

namespace PubNet.API.Controllers;

public abstract class PubNetControllerBase : ControllerBase
{
	protected NotFoundObjectResult NotFoundDto(string code, string message) => NotFound(new NotFoundErrorDto
	{
		Error = new GenericErrorContentDto
		{
			Code = code,
			Message = message,
		},
	});
}
