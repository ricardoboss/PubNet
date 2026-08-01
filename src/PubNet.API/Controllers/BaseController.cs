using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers;

public abstract class BaseController : ControllerBase
{
	/// <summary>
	/// Where the caller came from, used to point mails back at the frontend they used.
	/// </summary>
	protected Uri RefererUri => new(Request.Headers.Referer.FirstOrDefault() ?? Request.Host.ToString());

	[NonAction]
	protected ObjectResult FailedDependency(object? result)
	{
		return StatusCode(StatusCodes.Status424FailedDependency, result);
	}
}
