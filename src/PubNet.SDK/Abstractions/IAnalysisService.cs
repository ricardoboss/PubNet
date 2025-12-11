using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

public interface IAnalysisService
{
	Task<PackageVersionAnalysisDto?> GetAnalysisForPackageVersionAsync(string name, string version, bool includeReadme, CancellationToken cancellationToken = default);
}
