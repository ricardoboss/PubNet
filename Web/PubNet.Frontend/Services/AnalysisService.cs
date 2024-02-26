using PubNet.Client.Generated;
using PubNet.Client.Generated.Models;

namespace PubNet.Frontend.Services;

public class AnalysisService(ApiClient http, FetchLock<AnalysisService> fetchLock)
{
	private readonly Dictionary<string, Dictionary<(string, bool), DartPackageVersionAnalysisDto?>> _analyses =
		new Dictionary<string, Dictionary<(string, bool), DartPackageVersionAnalysisDto?>>();

	public void InvalidateAnalysisFor(string name, string? version = null)
	{
		if (!_analyses.TryGetValue(name, out var value))
			return;

		if (version is not null)
		{
			value.Remove((version, true));
			value.Remove((version, false));
		}
		else
		{
			_analyses.Remove(name);
		}
	}

	public async Task<DartPackageVersionAnalysisDto?> GetAnalysisForVersion(string name, string version, bool includeReadme, CancellationToken cancellationToken = default)
	{
		await fetchLock.UntilFreed(taskName: $"GetAnalysisForVersion({name}, {version}, {includeReadme})");

		if (_analyses.TryGetValue(name, out var versions))
		{
			if (versions.TryGetValue((version, includeReadme), out var value))
				return value;

			// fallback to returning an analysis including a readme, even if it was not requested
			if (includeReadme == false && versions.TryGetValue((version, true), out value))
				return value;
		}

		fetchLock.Lock($"GetAnalysisForVersion({name}, {version}, {includeReadme})");
		try
		{
			// TODO(rbo): incorporate includeReadme param
			var analysis = await http.Packages.Dart[name][version].AnalysisJson
				.GetAsync(cancellationToken: cancellationToken);

			if (!_analyses.ContainsKey(name))
				_analyses[name] = new Dictionary<(string, bool), DartPackageVersionAnalysisDto?>();

			_analyses[name][(version, includeReadme)] = analysis;

			return analysis;
		}
		finally
		{
			fetchLock.Free();
		}
	}
}
