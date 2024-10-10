using PubNet.Client.Abstractions;

namespace PubNet.Client.Extensions;

internal class DefaultPackagesQueryBuilder<TPackage, TPackageList>(IPackagesService<TPackage, TPackageList> service) : IPackagesQueryBuilder<TPackage, TPackageList>
{
	private string? _query;
	private int? _page;
	private int? _perPage;

	public IPackagesQueryBuilder<TPackage, TPackageList> WithQuery(string? query)
	{
		_query = query;

		return this;
	}

	public IPackagesQueryBuilder<TPackage, TPackageList> WithPage(int? page)
	{
		_page = page;

		return this;
	}

	public IPackagesQueryBuilder<TPackage, TPackageList> WithPerPage(int? perPage)
	{
		_perPage = perPage;

		return this;
	}

	public async Task<TPackageList> RunAsync(CancellationToken? cancellationToken = default)
	{
		return await service.GetPackagesAsync(_query, _page, _perPage, cancellationToken);
	}
}
