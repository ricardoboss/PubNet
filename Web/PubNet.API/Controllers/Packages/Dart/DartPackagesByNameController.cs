using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/{name}")]
[Tags("Dart")]
public class DartPackagesByNameController : DartController
{
	[HttpPatch("Discontinue")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> Discontinue(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
