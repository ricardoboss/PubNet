using System.Net.Http.Json;
using PubNet.API.DTO;

namespace PubNet.Frontend.Services;

public class AnalysisService
{
	private readonly ApiClient _http;
	private readonly Dictionary<(string, string, bool), PackageVersionAnalysisDto?> _analyses = new();

	private bool _fetching;

	public AnalysisService(ApiClient http)
	{
		_http = http;
	}

	public async Task<PackageVersionAnalysisDto?> GetAnalysisForVersion(string name, string version, bool includeReadme, CancellationToken cancellationToken = default)
	{
		while (_fetching) await Task.Delay(100, cancellationToken);

		if (_analyses.TryGetValue((name, version, includeReadme), out var value))
			return value;

		_fetching = true;
		try
		{
			var analysisResponse = await _http.GetAsync($"packages/{name}/versions/{version}/analysis?includeReadme={includeReadme}", cancellationToken);
			PackageVersionAnalysisDto? analysis;
			if (analysisResponse.IsSuccessStatusCode)
				analysis = await analysisResponse.Content.ReadFromJsonAsync<PackageVersionAnalysisDto>(cancellationToken: cancellationToken);
			else
				analysis = null;

			_analyses[(name, version, includeReadme)] = analysis;

			return analysis;
		}
		finally
		{
			_fetching = false;
		}
	}
}
