using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.DTO.Packages;

namespace PubNet.API.Controllers.Packages;

[ApiController]
[Route("Packages")]
[Tags("Packages")]
public class PackagesRootController(INugetPackageDao nugetPackageDao, IDartPackageDao dartPackageDao) : PackagesController
{
	[HttpGet("Search")]
	public async Task<PackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, PackageTypeFilter? type = null, CancellationToken cancellationToken = default)
	{
		var packages = Enumerable.Empty<PackageDto<PackageVersionDto>>();
		var totalHits = 0;

		type ??= PackageTypeFilter.Any;

		if (type.Value.HasFlag(PackageTypeFilter.Nuget))
		{
			var nugetHits = await nugetPackageDao.SearchAsync(q, skip, take, cancellationToken);

			totalHits += nugetHits.TotalHits;
			packages = packages.Concat(nugetHits.Packages.Cast<PackageDto<PackageVersionDto>>());
		}

		if (type.Value.HasFlag(PackageTypeFilter.Dart))
		{
			var dartHits = await dartPackageDao.SearchAsync(q, skip, take, cancellationToken);

			totalHits += dartHits.TotalHits;
			packages = packages.Concat(dartHits.Packages.Cast<PackageDto<PackageVersionDto>>());
		}

		return new()
		{
			TotalHits = totalHits,
			Packages = packages.OrderBy(x => x.Name),
		};
	}
}

[Flags]
public enum PackageTypeFilter
{
	None = 0,

	Nuget = 1 << 0,

	Dart = 1 << 1,

	Any = Nuget | Dart,
}
