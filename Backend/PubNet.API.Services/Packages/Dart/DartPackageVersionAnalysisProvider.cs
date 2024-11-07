using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.Services.Packages.Dart;

public class DartPackageVersionAnalysisProvider(IDartPackageDao packageDao) : IDartPackageVersionAnalysisProvider
{
	public async Task<DartPackageVersionAnalysis?> GetAnalysisAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		var package = await packageDao.GetByNameAsync(name, cancellationToken);
		var versionEntity = package?.Versions.FirstOrDefault(v => v.Version == version);
		var analysis = versionEntity?.Analysis;

		return analysis;
	}
}
