using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

public interface IPackagesService
{
	Task<PackageDto?> GetPackageAsync(string name, CancellationToken cancellationToken = default);

	Task DeletePackageAsync(string name, CancellationToken cancellationToken = default);

	Task<PackageVersionDto?> GetPackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default);

	Task DeletePackageVersionAsync(string name, string version, CancellationToken cancellationToken = default);

	Task DiscontinuePackageAsync(string name, string? replacement, CancellationToken cancellationToken = default);

	Task RetractPackageVersionAsync(string name, string version, CancellationToken cancellationToken = default);

	Task<AuthorPackagesResponse?> GetPackagesByAuthorAsync(string username,
		CancellationToken cancellationToken = default);

	Task<SearchPackagesResponse?> SearchPackagesAsync(CancellationToken cancellationToken = default);
}
