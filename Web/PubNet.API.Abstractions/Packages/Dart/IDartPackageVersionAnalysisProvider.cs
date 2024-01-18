using PubNet.Database.Entities.Dart;

namespace PubNet.API.Abstractions.Packages.Dart;

public interface IDartPackageVersionAnalysisProvider
{
	Task<DartPackageVersionAnalysis> GetAnalysisAsync(string name, string version, CancellationToken cancellationToken = default);
}
