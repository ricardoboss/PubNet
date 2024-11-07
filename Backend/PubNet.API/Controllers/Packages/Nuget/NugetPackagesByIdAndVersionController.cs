using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Attributes;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.Auth;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/{id}/Versions/{version}")]
[Tags("Nuget")]
[RequireAnyScope(Scopes.Packages.Nuget.Search, Scopes.Packages.Search)]
public class NugetPackagesByIdAndVersionController : NugetController
{
	[HttpGet]
	[ProducesResponseType<NugetPackageVersionDto>(StatusCodes.Status200OK)]
	public Task<NugetPackageVersionDto?> GetAsync(string id, string version,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	// [HttpGet("analysis.json")]
	// public Task<IActionResult> GetAnalysisAsync(string id, string version, CancellationToken cancellationToken = default)
	// {
	// 	throw new NotImplementedException();
	// }

	[HttpGet("archive.nupkg")]
	public Task<IActionResult> GetArchiveAsync(string id, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	// [HttpGet("Docs/{**path}")]
	// public Task<IActionResult> GetDocsAsync(string id, string version, string path, CancellationToken cancellationToken = default)
	// {
	// 	throw new NotImplementedException();
	// }

	[HttpDelete]
	[Authorize, RequireScope(Scopes.Packages.Nuget.Delete)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> DeleteAsync(string id, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
