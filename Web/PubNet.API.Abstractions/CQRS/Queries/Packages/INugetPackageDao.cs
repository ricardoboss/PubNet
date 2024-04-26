using PubNet.API.DTO.Packages.Nuget;
using PubNet.Database.Entities.Nuget;

namespace PubNet.API.Abstractions.CQRS.Queries.Packages;

public interface INugetPackageDao
{
	Task<NugetPackage?> TryGetByPackageIdAsync(string packageId, CancellationToken cancellationToken = default);

	Task<NugetPackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null,
		CancellationToken cancellationToken = default);
}
