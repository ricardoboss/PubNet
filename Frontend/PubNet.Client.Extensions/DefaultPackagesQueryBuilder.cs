using PubNet.Client.Abstractions;

namespace PubNet.Client.Extensions;

internal class DefaultPackagesQueryBuilder<TPackage, TPackageList>(IPackagesService<TPackage, TPackageList> service)
	: IPackagesQueryBuilder<TPackage, TPackageList>
{
	private string? currentQuery;
	private int? currentPage;
	private int? currentPerPage;

	public IPackagesQueryBuilder<TPackage, TPackageList> WithQuery(string? query)
	{
		currentQuery = query;

		return this;
	}

	public IPackagesQueryBuilder<TPackage, TPackageList> WithPage(int? page)
	{
		currentPage = page;

		return this;
	}

	public IPackagesQueryBuilder<TPackage, TPackageList> WithPerPage(int? perPage)
	{
		currentPerPage = perPage;

		return this;
	}

	public async Task<TPackageList> RunAsync(CancellationToken? cancellationToken = default)
	{
		return await service.GetPackagesAsync(currentQuery, currentPage, currentPerPage, cancellationToken);
	}
}
