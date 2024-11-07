using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface IDartPackagesService : IPackagesService<DartPackageDto, DartPackageVersionDto, DartPackageListDto>
{
	Task<DartPackageVersionAnalysisDto?> GetPackageVersionAnalysisAsync(string name, string version,
		CancellationToken cancellationToken = default);
}
