using PubNet.API.DTO.Packages.Dart;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.Abstractions.CQRS.Queries.Packages;

public interface IDartPackageDao
{
	Task<DartPackage?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

	Task<DartPackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null,
		CancellationToken cancellationToken = default);
}
