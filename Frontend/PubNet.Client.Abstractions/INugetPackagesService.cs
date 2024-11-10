using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface INugetPackagesService : IPackagesService<NugetPackageDto, NugetPackageVersionDto, NugetPackageListDto>;
