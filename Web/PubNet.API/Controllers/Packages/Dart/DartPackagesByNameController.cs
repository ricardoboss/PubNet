using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Dart;

[ApiController]
[Route("packages/dart/{name}")]
public class DartPackagesByNameController : ControllerBase
{
	[HttpPatch("discontinue")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> Discontinue(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
