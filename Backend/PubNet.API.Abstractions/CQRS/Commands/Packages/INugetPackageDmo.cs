using PubNet.Database.Entities;
using PubNet.Database.Entities.Nuget;

namespace PubNet.API.Abstractions.CQRS.Commands.Packages;

public interface INugetPackageDmo
{
	Task<NugetPackageVersion> CreateAsync(Author author, byte[] nupkg, CancellationToken cancellationToken = default);

	Task<NugetPackageVersion> AddVersionAsync(NugetPackage package, byte[] nupkg, CancellationToken cancellationToken = default);
}
