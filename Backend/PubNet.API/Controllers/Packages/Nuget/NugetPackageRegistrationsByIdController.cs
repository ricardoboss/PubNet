using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/Registrations/{id}")]
[Tags("Nuget")]
public class NugetPackageRegistrationsByIdController(INugetPackageDao nugetPackageDao) : NugetController
{
	/// <summary>
	/// Used by dotnet to gather information about a specific package.
	/// </summary>
	/// <param name="id">The package ID</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A <see cref="NugetPackageRegistrationIndexDto"/> that contains the information needed to upload the package.</returns>
	[HttpGet("index.json")]
	[ProducesResponseType<NugetPackageRegistrationIndexDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetPackageRegistrationsIndexAsync(string id,
		CancellationToken cancellationToken = default)
	{
		var package = await nugetPackageDao.TryGetByPackageIdAsync(id, cancellationToken);

		return package is null
			? NotFoundDto("package-not-found", "Package not found")
			: Ok(NugetPackageRegistrationIndexDto.MapFrom(package));
	}
}
