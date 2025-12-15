using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class ApiAnalysisService(PubNetApiClient apiClient) : IAnalysisService
{
	public async Task<PackageVersionAnalysisDto?> GetAnalysisForPackageVersionAsync(string name, string version,
		bool includeReadme, CancellationToken cancellationToken = default)
	{
		return await apiClient.Packages[name].Versions[version].Analysis
			.GetAsync(r => r.QueryParameters.IncludeReadme = includeReadme, cancellationToken: cancellationToken);
	}
}
