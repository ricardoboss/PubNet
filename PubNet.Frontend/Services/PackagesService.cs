using System.Net;
using System.Net.Http.Json;
using PubNet.API.DTO;

namespace PubNet.Frontend.Services;

public class PackagesService
{
	private readonly ApiClient _http;
	private readonly Dictionary<string, PackageDto?> _packages = new();

	private bool _fetching;

	public PackagesService(ApiClient http)
	{
		_http = http;
	}

	private static PackageNotFoundException NotFound => new("The package you are looking for does not exist");
	private static PackageFetchException FetchException(string message) => new(message);

	public async Task<PackageDto?> GetPackage(string name, bool includeAuthor, CancellationToken cancellationToken = default)
	{
		while (_fetching) await Task.Delay(100, cancellationToken);

		if (_packages.TryGetValue(name, out var package))
			return package ?? throw NotFound;

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

			if (response.StatusCode != HttpStatusCode.NotFound) throw FetchException("An undocumented error occurred");

			_packages[name] = null;

			throw NotFound;
		}
		finally
		{
			_fetching = false;
		}
	}

	public async Task<PackageVersionDto?> GetPackageVersion(string name, string version)
	{
		return (await GetPackage(name, false))?.Versions?.FirstOrDefault(v => v.Version == version);
	}
}

public class PackageFetchException : Exception
{
	public PackageFetchException(string message) : base(message)
	{
	}
}

public class PackageNotFoundException : Exception
{
	public PackageNotFoundException(string message) : base(message)
	{
	}
}
