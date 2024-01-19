using PubNet.API.Abstractions.Packages.Nuget;

namespace PubNet.API.Services.Packages.Nuget;

public class KnownUrlsProvider(IHttpContextAccessor contextAccessor) : IKnownUrlsProvider
{
	private string GetBaseUrl()
	{
		var request = contextAccessor.HttpContext?.Request;
		if (request is null)
		{
			throw new InvalidOperationException("HttpContext is null");
		}

		var scheme = request.Scheme;
		var host = request.Host;
		var pathBase = request.PathBase;
		return $"{scheme}://{host}{pathBase}";
	}

	/// <inheritdoc />
	public string GetRegistrationsBaseUrl()
	{
		return GetBaseUrl() + "/Packages/Nuget/Registrations";
	}

	/// <inheritdoc />
	public string GetPackageBaseAddress()
	{
		return GetBaseUrl() + "/Packages/Nuget/Package";
	}

	/// <inheritdoc />
	public string GetPackagePublishUrl()
	{
		return GetBaseUrl() + "/Package/Nuget/Publish";
	}
}
