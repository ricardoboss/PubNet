using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Attributes;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Web;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/{name}")]
[Tags("Dart")]
public class DartPackagesByNameController(IDartPackageDmo dartPackageDmo, IDartPackageDao dartPackageDao) : DartController
{
	[HttpPatch("Discontinue")]
	[Authorize, RequireScope(Scopes.Dart.Discontinue)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<DartSuccessDto> Discontinue(string name, CancellationToken cancellationToken = default)
	{
		await dartPackageDmo.DiscontinueAsync(name, cancellationToken);

		return DartSuccessDto.WithMessage($"Package '{name}' has been discontinued");
	}

	[HttpGet]
	public async Task<DartPackageIndexDto?> GetAsync(string name, CancellationToken cancellationToken = default)
	{
		var package = await dartPackageDao.TryGetByNameAsync(name, cancellationToken);

		return package is null ? null : DartPackageIndexDto.MapFrom(package);
	}
}
