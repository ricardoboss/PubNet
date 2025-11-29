using System.Net.Http.Json;
using PubNet.API.DTO;

namespace PubNet.Frontend.Services;

public class AnalysisService(ApiClient http, FetchLock<AnalysisService> fetchLock)
{
	private readonly Dictionary<string, Dictionary<(string, bool), PackageVersionAnalysisDto?>> _analyses = new();

	public void InvalidateAnalysisFor(string name, string? version = null)
	{
		if (!_analyses.ContainsKey(name))
			return;

		if (version is not null)
		{
			_analyses[name].Remove((version, true));
			_analyses[name].Remove((version, false));
		}
		else
		{
			_analyses.Remove(name);
		}
	}

	public async Task<PackageVersionAnalysisDto?> GetAnalysisForVersion(string name, string version, bool includeReadme, CancellationToken cancellationToken = default)
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
			var analysisResponse = await http.GetAsync($"packages/{name}/versions/{version}/analysis?includeReadme={includeReadme}", cancellationToken);
			PackageVersionAnalysisDto? analysis;
			if (analysisResponse.IsSuccessStatusCode)
				analysis = await analysisResponse.Content.ReadFromJsonAsync<PackageVersionAnalysisDto>(cancellationToken: cancellationToken);
			else
				analysis = null;

			if (!_analyses.ContainsKey(name))
				_analyses[name] = new();

			_analyses[name][(version, includeReadme)] = analysis;

			return analysis;
		}
		finally
		{
			fetchLock.Free();
		}
	}
}
