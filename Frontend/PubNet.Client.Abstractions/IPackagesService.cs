namespace PubNet.Client.Abstractions;

public interface IPackagesService<TPackage, TPackageList>
{
	Task<TPackage> GetPackageAsync(string name, CancellationToken cancellationToken = default);

	Task<TPackageList> GetPackagesAsync(string? query = null, int? page = null, int? perPage = null,
		CancellationToken? cancellationToken = default);
}
