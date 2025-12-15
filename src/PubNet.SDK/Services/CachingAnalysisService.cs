using Microsoft.Extensions.Logging;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class CachingAnalysisService(IAnalysisService inner, ILogger<CachingAnalysisService> logger) : IAnalysisService
{
	private readonly Dictionary<(string name, string version, bool includeReadme), PackageVersionAnalysisDto>
		cache = new();

	public async Task<PackageVersionAnalysisDto?> GetAnalysisForPackageVersionAsync(string name, string version,
		bool includeReadme, CancellationToken cancellationToken = default)
	{
		if (cache.TryGetValue((name, version, includeReadme), out var analysis))
		{
			logger.LogTrace(
				"Providing cached analysis for {{ name={PackageName}, version={PackageVersion}, includeReadme={IncludeReadme} }}",
				name, version, includeReadme);

			return analysis;
		}

		logger.LogTrace(
			"Cache miss for {{ name={PackageName}, version={PackageVersion}, includeReadme={IncludeReadme} }}",
			name, version, includeReadme);

		analysis = await inner.GetAnalysisForPackageVersionAsync(name, version, includeReadme, cancellationToken);

		if (analysis is not null)
		{
			logger.LogTrace(
				"Caching analysis for {{ name={PackageName}, version={PackageVersion}, includeReadme={IncludeReadme} }}",
				name, version, includeReadme);

			cache[(name, version, includeReadme)] = analysis;
		}

		return analysis;
	}
}
