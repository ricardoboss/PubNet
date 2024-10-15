using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/Registrations/{id}")]
[Tags("Nuget")]
public class NugetPackageRegistrationsByIdController(INugetPackageDao nugetPackageDao) : NugetController
{
	/// <summary>
	/// Used by dotnet to gather information about a specific package.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	[HttpGet("index.json")]
	public async Task<NugetPackageRegistrationIndexDto?> GetPackageRegistrationsIndexAsync(string id, CancellationToken cancellationToken = default)
	{
		var package = await nugetPackageDao.TryGetByPackageIdAsync(id, cancellationToken);

		return package is null ? null : NugetPackageRegistrationIndexDto.FromNugetPackage(package);
	}
}
