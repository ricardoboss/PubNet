using PubNet.API.Abstractions.Packages.Nuget;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Services.Packages.Nuget;

public class NugetServiceIndexProvider(IKnownUrlsProvider knownUrlsProvider) : INugetServiceIndexProvider
{
	/// <inheritdoc />
	public Task<NugetServiceIndexDto> GetServiceIndexAsync(CancellationToken cancellationToken = default)
	{
		var resources = GetResourceTypesMaps()
			.Select(map => new NugetServiceIndexResourceDto
			{
				Id = map.Id,
				Type = map.Type,
				Comment = map.Comment,
			});

		var dto = new NugetServiceIndexDto
		{
			Resources = resources,
		};

		return Task.FromResult(dto);
	}

	private record ResourceTypesMap(string Id, string Type, string? Comment = null);

	private IEnumerable<ResourceTypesMap> GetResourceTypesMaps()
	{
		yield return new ResourceTypesMap(
			knownUrlsProvider.GetRegistrationsBaseUrl(),
			"RegistrationsBaseUrl/3.6.0",
			"Includes SemVer 2.0.0 packages"
		);

		yield return new ResourceTypesMap(
			knownUrlsProvider.GetPackageBaseAddress(),
			"PackageBaseAddress/3.0.0",
			"The initial release"
		);

		yield return new ResourceTypesMap(
			knownUrlsProvider.GetPackagePublishUrl(),
			"PackagePublish/2.0.0",
			"The initial release"
		);

		// yield return new ResourceTypesMap(
		// 	knownUrlsProvider.GetSearchAutocompleteServiceUrl(),
		// 	"SearchAutocompleteService/3.5.0",
		// 	"Includes support for packageType query parameter"
		// );

		// yield return new ResourceTypesMap(
		// 	knownUrlsProvider.GetSearchQueryServiceUrl(),
		// 	"SearchQueryService/3.5.0",
		// 	"Includes support for packageType query parameter"
		// );

		// yield return new ResourceTypesMap(
		// 	knownUrlsProvider.GetVulnerabilityInfoUrl(),
		// 	"VulnerabilityInfo/6.7.0",
		// 	"The initial release"
		// );
	}
}
