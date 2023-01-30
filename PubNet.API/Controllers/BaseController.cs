using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers;

public abstract class BaseController : ControllerBase
{
    [NonAction]
    protected ObjectResult FailedDependency(object? result) => StatusCode(StatusCodes.Status424FailedDependency, result);
}
