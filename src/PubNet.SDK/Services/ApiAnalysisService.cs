using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Helpers;

namespace PubNet.SDK.Services;

public class ApiAnalysisService(PubNetApiClient apiClient, FetchLock<ApiAnalysisService> fetchLock) : IAnalysisService
{
	private readonly Dictionary<string, Dictionary<string, PackageVersionAnalysisDto?>> _analyses = new();

	public async Task<PackageVersionAnalysisDto?> GetAnalysisForPackageVersionAsync(string name, string version,
		bool includeReadme, CancellationToken cancellationToken = default)
	{
		await fetchLock.UntilFreed(taskName: $"GetAnalysisForPackageVersionAsync({name}, {version}, {includeReadme})");

		if (_analyses.TryGetValue(name, out var versions))
		{
			if (versions.TryGetValue(version, out var value))
				return value;
		}

		using var _ = fetchLock.Lock($"GetAnalysisForPackageVersionAsync({name}, {version}, {includeReadme})");

		var analysis = await apiClient.Packages[name].Versions[version].Analysis
			.GetAsync(r => r.QueryParameters.IncludeReadme = includeReadme, cancellationToken: cancellationToken);
		if (analysis is null)
			return null;

		if (!_analyses.ContainsKey(name))
			_analyses[name] = new();

		_analyses[name][version] = analysis;

		return analysis;
	}
}
