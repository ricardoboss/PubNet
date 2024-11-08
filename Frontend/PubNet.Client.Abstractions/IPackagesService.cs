namespace PubNet.Client.Abstractions;

public interface IPackagesService<TPackage, TPackageVersion, TPackageList>
{
	Task<TPackage?> GetPackageAsync(string name, CancellationToken cancellationToken = default);

	Task<TPackageVersion?> GetPackageVersionAsync(string name, string? version, CancellationToken cancellationToken = default);

	Task<TPackageList> GetPackagesAsync(string? query = null, int? page = null, int? perPage = null,
		CancellationToken? cancellationToken = default);

	Task<TPackageList> ByAuthorAsync(string author, string? query = null, int? page = null, int? perPage = null,
		CancellationToken? cancellationToken = default);
}
