using PubNet.Database.Entities;
using PubNet.Database.Entities.Dart;
using Semver;

namespace PubNet.API.Abstractions.CQRS.Commands.Packages;

public interface IDartPackageDmo
{
	Task<DartPackageVersion> CreateAsync(string name, Author author, SemVersion initialVersion, PubSpec pubspec, CancellationToken cancellationToken = default);

	Task RetractAsync(string name, string version, CancellationToken cancellationToken = default);

	Task DiscontinueAsync(string name, CancellationToken cancellationToken = default);

	Task SaveLatestVersionAsync(DartPackage package, DartPackageVersion version, CancellationToken cancellationToken = default);
}
