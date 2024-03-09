using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.Database.Context;
using PubNet.Database.Entities.Nuget;

namespace PubNet.API.Services.CQRS.Queries.Packages;

public class NugetPackageDao(PubNetContext dbContent) : INugetPackageDao
{
	public Task<NugetPackage?> TryGetByPackageIdAsync(string packageId, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(dbContent.NugetPackages.SingleOrDefault(p => p.Name == packageId));
	}
}
