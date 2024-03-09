using PubNet.Database.Entities.Nuget;

namespace PubNet.API.Abstractions.CQRS.Queries.Packages;

public interface INugetPackageDao
{
	Task<NugetPackage?> TryGetByPackageIdAsync(string packageId, CancellationToken cancellationToken = default);
}
