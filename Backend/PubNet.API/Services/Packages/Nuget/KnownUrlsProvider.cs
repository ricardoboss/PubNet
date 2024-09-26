using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Packages.Nuget;
using PubNet.API.Controllers.Packages.Nuget;

namespace PubNet.API.Services.Packages.Nuget;

public class KnownUrlsProvider(IHttpContextAccessor contextAccessor, IActionTemplateGenerator actionTemplateGenerator) : IKnownUrlsProvider
{
	private readonly Lazy<string> baseUrl = new(() =>
	{
		var request = contextAccessor.HttpContext?.Request;
		if (request is null)
		{
			throw new InvalidOperationException("HttpContext is null");
		}

		var scheme = request.Scheme;
		var host = request.Host;
		var pathBase = request.PathBase;
		return $"{scheme}://{host}{pathBase}/";
	});

	private string BaseUrl => baseUrl.Value;

	/// <inheritdoc />
	public string GetRegistrationsBaseUrl()
	{
		var route = actionTemplateGenerator.GetActionRoute(
			nameof(NugetPackageRegistrationsByIdController),
			nameof(NugetPackageRegistrationsByIdController.GetPackageRegistrationsIndexAsync)
		);

		return BaseUrl + route[..^"/{id}/index.json".Length];
	}

	/// <inheritdoc />
	public string GetPackageBaseAddress()
	{
		var route = actionTemplateGenerator.GetActionRoute(
			nameof(NugetPackageByIdController),
			nameof(NugetPackageByIdController.GetPackageIndexAsync)
		);

		return BaseUrl + route[..^"/{id}/index.json".Length];
	}

	/// <inheritdoc />
	public string GetPackagePublishUrl()
	{
		var route = actionTemplateGenerator.GetActionRoute(
			nameof(NugetRootController),
			nameof(NugetRootController.PublishAsync)
		);

		return BaseUrl + route;
	}

	/// <inheritdoc />
	public string GetSearchAutocompleteServiceUrl()
	{
		var route = actionTemplateGenerator.GetActionRoute(
			nameof(NugetRootController),
			nameof(NugetRootController.AutocompleteAsync)
		);

		return BaseUrl + route;
	}

	/// <inheritdoc />
	public string GetSearchQueryServiceUrl()
	{
		var route = actionTemplateGenerator.GetActionRoute(
			nameof(NugetRootController),
			nameof(NugetRootController.SearchAsync)
		);

		return BaseUrl + route;
	}

	/// <inheritdoc />
	public string GetVulnerabilityInfoUrl()
	{
		var route = actionTemplateGenerator.GetActionRoute(
			nameof(NugetRootController),
			nameof(NugetRootController.GetVulnerabilitiesAsync)
		);

		return BaseUrl + route;
	}
}
