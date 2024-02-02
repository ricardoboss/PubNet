using PubNet.Database.Entities.Dart;

namespace PubNet.API.Abstractions.CQRS.Queries.Packages;

public interface IDartPackageDao
{
	Task<DartPackage?> TryGetByNameAsync(string name, CancellationToken cancellationToken = default);
}
