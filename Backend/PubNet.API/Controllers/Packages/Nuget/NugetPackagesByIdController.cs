using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/{id}")]
[Tags("Nuget")]
public class NugetPackageByIdController(INugetPackageDao packageDao) : NugetController
{
	/// <summary>
	/// Used by dotnet to gather information about a specific package.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[HttpGet("index.json")]
	[ProducesResponseType<NugetPackageIndexDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetPackageIndexAsync(string id, CancellationToken cancellationToken = default)
	{
		var package = await packageDao.TryGetByPackageIdAsync(id, cancellationToken);

		return package is null
			? NotFoundDto("package-not-found", $"Package '{id}' not found")
			: Ok(new NugetPackageIndexDto
			{
				Versions = package.Versions.Select(v => v.Version),
			});
	}

	/// <summary>
	/// Used by dotnet to gather information about a specific package.
	/// </summary>
	/// <param name="id">The package ID</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A <see cref="NugetPackageDto"/> that contains the package information.</returns>
	[HttpGet]
	[ProducesResponseType<NugetPackageDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetPackageAsync(string id, CancellationToken cancellationToken = default)
	{
		var package = await packageDao.TryGetByPackageIdAsync(id, cancellationToken);

		return package is null
			? NotFoundDto("package-not-found", $"Package '{id}' not found")
			: Ok(NugetPackageDto.MapFrom(package));
	}
}
