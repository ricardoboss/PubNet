using PubNet.API.DTO;

namespace PubNet.API.Services;

public class PubDevPackageProvider
{
	public const string ClientName = "PubDev";

	private readonly IHttpClientFactory _clientFactory;
	private readonly Dictionary<string, PackageDto> _packageCache = new();

	public PubDevPackageProvider(IHttpClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
	}

	public async Task<PackageVersionDto?> TryGetVersion(string name, string version,
		CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		using var client = _clientFactory.CreateClient(ClientName);

		try
		{
			var dto = await client.GetFromJsonAsync<PackageVersionDto>($"/api/packages/{name}/versions/{version}",
				cancellationToken);
			if (dto is null) return null;

			dto.Mirrored = true;

			return dto;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public async Task<PackageDto?> TryGetPackage(string name, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (_packageCache.TryGetValue(name, out var existingDto)) return existingDto;

		using var client = _clientFactory.CreateClient(ClientName);

		try
		{
			var dto = await client.GetFromJsonAsync<PackageDto>($"/api/packages/{name}", cancellationToken);
			if (dto is null) return null;

			dto.Mirrored = true;
			_packageCache[name] = dto;

			return dto;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
