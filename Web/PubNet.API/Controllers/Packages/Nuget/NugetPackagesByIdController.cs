using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/{id}")]
[Tags("Nuget")]
public class NugetPackagesByIdController : NugetController
{
	[HttpGet("index.json")]
	public Task<NugetPackageIndexDto> GetPackageIndexAsync(string id, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
