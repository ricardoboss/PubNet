using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/{id}")]
[Tags("Nuget")]
public class NugetPackagesByIdController : NugetController
{
	[HttpGet("index.json")]
	public Task<IActionResult> GetPackageAsync(string id, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
