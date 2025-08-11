using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Attributes;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Auth;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/{name}")]
[Tags("Dart")]
public class DartPackagesByNameController(
	IDartPackageDmo dartPackageDmo,
	IDartPackageDao dartPackageDao,
	IDartPackageArchiveProvider archiveProvider
) : DartController
{
	[HttpPatch("Discontinue")]
	[RequireScope(Scopes.Packages.Dart.Discontinue)]
	public async Task<DartSuccessDto> Discontinue(string name, CancellationToken cancellationToken = default)
	{
		await dartPackageDmo.DiscontinueAsync(name, cancellationToken);

		return DartSuccessDto.WithMessage("package_discontinued", $"Package '{name}' has been discontinued");
	}

	[HttpGet]
	[ProducesResponseType<DartPackageDto>(200)]
	[ProducesResponseType<NotFoundErrorDto>(404)]
	public async Task<IActionResult> GetAsync(string name, CancellationToken cancellationToken = default)
	{
		var package = await dartPackageDao.GetByNameAsync(name, cancellationToken);

		if (package is null)
			return NotFoundDto("package-not-found", $"Package '{name}' not found");

		return Ok(DartPackageDto.MapFrom(package, archiveProvider.GetArchiveUriAndHash));
	}
}
