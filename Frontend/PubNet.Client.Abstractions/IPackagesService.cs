namespace PubNet.Client.Abstractions;

public interface IPackagesService<TPackage, TPackageList>
{
	Task<TPackageList> GetPackagesAsync(string? query = null, int? page = null, int? perPage = null,
		CancellationToken? cancellationToken = default);
}
