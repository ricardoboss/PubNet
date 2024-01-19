using PubNet.API.Abstractions.Packages.Nuget;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Services.Packages.Nuget;

public class NugetServiceIndexProvider(IKnownUrlsProvider knownUrlsProvider) : INugetServiceIndexProvider
{
	/// <inheritdoc />
	public Task<NugetServiceIndexDto> GetServiceIndexAsync(CancellationToken cancellationToken = default)
	{
		return Task.FromResult<NugetServiceIndexDto>(new()
		{
			Resources = GetResources(),
		});
	}

	private IEnumerable<NugetServiceIndexResourceDto> GetResources()
	{
		return from map in GetResourceTypesMaps()
			from alias in map.Aliases
			select new NugetServiceIndexResourceDto
			{
				Id = map.Id,
				Type = alias.Type,
				Comment = alias.Comment,
			};
	}

	private record ResourceTypesMap(string Id, (string Type, string? Comment)[] Aliases);

	private IEnumerable<ResourceTypesMap> GetResourceTypesMaps()
	{
		yield return new(
			knownUrlsProvider.GetRegistrationsBaseUrl(),
			[
				("RegistrationsBaseUrl/3.6.0", "Includes SemVer 2.0.0 packages"),
			]
		);

		yield return new(
			knownUrlsProvider.GetPackageBaseAddress(),
			[
				("PackageBaseAddress/3.0.0", "The initial release"),
			]
		);

		yield return new(
			knownUrlsProvider.GetPackagePublishUrl(),
			[
				("PackagePublish/2.0.0", "The initial release"),
			]
		);
	}
}
