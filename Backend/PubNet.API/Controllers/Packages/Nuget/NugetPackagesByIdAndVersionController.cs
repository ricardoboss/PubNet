using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Attributes;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.Auth;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/{id}/Versions/{version}")]
[Tags("Nuget")]
public class NugetPackagesByIdAndVersionController(INugetPackageDao packageDao) : NugetController
{
	[HttpGet]
	[ProducesResponseType<NugetPackageVersionDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAsync(string id, string version,
		CancellationToken cancellationToken = default)
	{
		var package = await packageDao.TryGetByPackageIdAsync(id, cancellationToken);
		if (package is null)
			return NotFoundDto("package-not-found", $"Package '{id}' not found");

		var matchingVersion = string.Equals(version, "latest", StringComparison.OrdinalIgnoreCase)
			? package.LatestVersion
			: package.Versions.FirstOrDefault(v => v.Version == version);

		if (matchingVersion is null)
			return NotFoundDto("package-version-not-found", $"Package '{id}' version '{version}' not found");

		return Ok(NugetPackageVersionDto.MapFrom(matchingVersion));
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
