using PubNet.API.Abstractions.Packages.Dart;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.Services.Packages.Dart;

public class DartPackageVersionAnalysisProvider : IDartPackageVersionAnalysisProvider
{
	public async Task<DartPackageVersionAnalysis?> GetAnalysisAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
