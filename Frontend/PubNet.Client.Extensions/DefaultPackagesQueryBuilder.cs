using PubNet.Client.Abstractions;

namespace PubNet.Client.Extensions;

internal class DefaultPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>(
	IPackagesService<TPackage, TPackageVersion, TPackageList> service)
	: IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>
{
	private string? currentAuthor;
	private string? currentQuery;
	private int? currentPage;
	private int? currentPerPage;

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> ByAuthor(string author)
	{
		currentAuthor = author;

		return this;
	}

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithQuery(string? query)
	{
		currentQuery = query;

		return this;
	}

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithPage(int? page)
	{
		currentPage = page;

		return this;
	}

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithPerPage(int? perPage)
	{
		currentPerPage = perPage;

		return this;
	}

	public async Task<TPackageList> RunAsync(CancellationToken? cancellationToken = default)
	{
		if (currentAuthor is not null)
			return await service.ByAuthorAsync(currentAuthor, currentQuery, currentPage, currentPerPage,
				cancellationToken);

		return await service.GetPackagesAsync(currentQuery, currentPage, currentPerPage, cancellationToken);
	}
}
