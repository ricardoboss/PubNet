namespace PubNet.Client.Extensions;

public interface IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>
{
	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> ByAuthor(string author);

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithQuery(string? query);

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithPage(int? page);

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithPerPage(int? perPage);

	public Task<TPackageList> RunAsync(CancellationToken cancellationToken = default);
}
