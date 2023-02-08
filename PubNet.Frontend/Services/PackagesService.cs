using System.Net;
using System.Net.Http.Json;
using PubNet.API.DTO;

namespace PubNet.Frontend.Services;

public class PackagesService
{
	private const int FetchDelay = 10;

	private readonly ApiClient _http;
	private readonly AnalysisService _analysis;
	private readonly Dictionary<string, PackageDto?> _packages = new();

	private bool _fetching;

	public PackagesService(ApiClient http, AnalysisService analysis)
	{
		_http = http;
		_analysis = analysis;
	}

	private static UnauthorizedAccessException Unauthorized(string message) => new(message);
	private static PackageNotFoundException NotFound(string? message = null) => new(message ?? "The package you are looking for does not exist");
	private static PackageFetchException Undocumented => new("An undocumented error occurred");
	private static ApiServerException InternalServerError => new("An internal server error occurred");

	public async Task<PackageDto?> GetPackage(string name, bool includeAuthor, bool forceFetch = false, CancellationToken cancellationToken = default)
	{
		while (_fetching) await Task.Delay(FetchDelay, cancellationToken);

		if (!forceFetch && _packages.TryGetValue(name, out var package))
			return package ?? throw NotFound();

		_fetching = true;
		try
		{
			var response = await _http.GetAsync($"packages/{name}?includeAuthor={includeAuthor}", cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				package = await response.Content.ReadFromJsonAsync<PackageDto>(cancellationToken: cancellationToken);

				_packages[name] = package;

				return package;
			}

			_packages[name] = null;

			throw response.StatusCode switch
			{
				HttpStatusCode.NotFound => NotFound(),
				HttpStatusCode.InternalServerError => InternalServerError,
				_ => Undocumented,
			};
		}
		finally
		{
			_fetching = false;
		}
	}

	public async Task DeletePackage(string name, CancellationToken cancellationToken = default)
	{
		while (_fetching) await Task.Delay(FetchDelay, cancellationToken);

		_fetching = true;
		try
		{
			var response = await _http.DeleteAsync($"packages/{name}", cancellationToken);
			if (response.IsSuccessStatusCode) return;

			throw response.StatusCode switch
			{
				HttpStatusCode.Unauthorized => Unauthorized("You are not authorized to delete this package"),
				HttpStatusCode.NotFound => NotFound("The package you are trying to delete was not found"),
				HttpStatusCode.InternalServerError => InternalServerError,
				_ => Undocumented,
			};
		}
		finally
		{
			// delete package from cache, regardless of API call result, so it has to be re-fetched
			_packages.Remove(name);

			_analysis.InvalidateAnalysisFor(name);

			_fetching = false;
		}
	}

	public async Task DeletePackageVersion(string name, string version, CancellationToken cancellationToken = default)
	{
		while (_fetching) await Task.Delay(FetchDelay, cancellationToken);

		_fetching = true;
		try
		{
			var response = await _http.DeleteAsync($"packages/{name}/versions/{version}", cancellationToken);
			if (response.IsSuccessStatusCode) return;

			throw response.StatusCode switch
			{
				HttpStatusCode.Unauthorized => Unauthorized("You are not authorized to delete this package version"),
				HttpStatusCode.NotFound => NotFound("The package version you are trying to delete was not found"),
				HttpStatusCode.InternalServerError => InternalServerError,
				_ => Undocumented,
			};
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package) && package?.Versions != null)
			{
				var versionIdx = package.Versions.FindIndex(v => v.Version == version);

				package.Versions.RemoveAt(versionIdx);
			}

			_analysis.InvalidateAnalysisFor(name, version);

			_fetching = false;
		}
	}

	public async Task<PackageVersionDto?> GetPackageVersion(string name, string version)
	{
		return (await GetPackage(name, false))?.Versions?.FirstOrDefault(v => v.Version == version);
	}

	public async Task DiscontinuePackage(string name, string? replacement, CancellationToken cancellationToken = default)
	{
		while (_fetching) await Task.Delay(FetchDelay, cancellationToken);

		_fetching = true;
		try
		{
			var dto = new SetDiscontinuedDto(string.IsNullOrWhiteSpace(replacement) ? null : replacement);

			var response = await _http.PatchAsync($"packages/{name}/discontinue", dto, cancellationToken);
			if (response.IsSuccessStatusCode) return;

			throw response.StatusCode switch
			{
				HttpStatusCode.Unauthorized => Unauthorized("You are not authorized to discontinue this package"),
				HttpStatusCode.NotFound => NotFound(),
				HttpStatusCode.InternalServerError => InternalServerError,
				_ => Undocumented,
			};
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package) && package is not null)
			{
				package.IsDiscontinued = true;
			}

			_analysis.InvalidateAnalysisFor(name);

			_fetching = false;
		}
	}

	public async Task RetractVersion(string name, string version, CancellationToken cancellationToken = default)
	{
		while (_fetching) await Task.Delay(FetchDelay, cancellationToken);

		_fetching = true;
		try
		{
			var response = await _http.PatchAsync($"packages/{name}/versions/{version}/retract", cancellationToken);
			if (response.IsSuccessStatusCode) return;

			throw response.StatusCode switch
			{
				HttpStatusCode.Unauthorized => Unauthorized("You are not authorized to retract this package version"),
				HttpStatusCode.NotFound => NotFound("The package version you are trying to retract does not exist"),
				HttpStatusCode.InternalServerError => InternalServerError,
				_ => Undocumented,
			};
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package))
			{
				var dto = package?.Versions?.FirstOrDefault(v => v.Version == version);
				if (dto is not null) dto.Retracted = true;
			}

			_analysis.InvalidateAnalysisFor(name, version);

			_fetching = false;
		}
	}
}
