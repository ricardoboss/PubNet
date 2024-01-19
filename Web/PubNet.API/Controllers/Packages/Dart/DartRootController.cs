using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Dart;

[ApiController]
[Route("packages/dart")]
public class DartRootController : ControllerBase
{
	[HttpPost("create")]
	[ProducesResponseType(StatusCodes.Status302Found)]
	public Task<IActionResult> CreateNewAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("search")]
	public Task<IActionResult> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
