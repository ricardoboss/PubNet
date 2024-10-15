namespace PubNet.Client.Extensions;

public interface IPackagesQueryBuilder<TPackage, TPackageList>
{
	public IPackagesQueryBuilder<TPackage, TPackageList> WithQuery(string? query);

	public IPackagesQueryBuilder<TPackage, TPackageList> WithPage(int? page);

	public IPackagesQueryBuilder<TPackage, TPackageList> WithPerPage(int? perPage);

	public Task<TPackageList> RunAsync(CancellationToken? cancellationToken = default);
}
