using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Abstractions.Packages.Nuget;

public interface INugetServiceIndexProvider
{
	public Task<NugetServiceIndexDto> GetServiceIndexAsync(CancellationToken cancellationToken = default);
}
