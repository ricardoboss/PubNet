using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Attributes;
using PubNet.API.DTO;
using PubNet.API.DTO.Packages;
using PubNet.Auth;

namespace PubNet.API.Controllers.Packages;

[ApiController]
[Route("Packages")]
[Tags("Packages")]
public class PackagesRootController(INugetPackageDao nugetPackageDao, IDartPackageDao dartPackageDao) : PackagesController
{
	[HttpGet("Search")]
	[Authorize, RequireScope(Scopes.Packages.Search)]
	[ProducesResponseType<PackageListCollectionDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<PackageListCollectionDto> SearchAsync(string? q = null, int? skipDart = null, int? takeDart = null, int? skipNuget = null, int? takeNuget = null, CancellationToken cancellationToken = default)
	{
		var dartHits = await dartPackageDao.SearchAsync(q, skipDart, takeDart, cancellationToken);
		var nugetHits = await nugetPackageDao.SearchAsync(q, skipNuget, takeNuget, cancellationToken);

		return new()
		{
			Dart = dartHits,
			Nuget = nugetHits,
		};
	}
}
