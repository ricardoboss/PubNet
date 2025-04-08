using PubNet.Client.Abstractions;

namespace PubNet.Client.Extensions;

internal class
	DefaultPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> : IPackagesQueryBuilder<TPackage,
	TPackageVersion, TPackageList>
{
	private readonly string? currentAuthor;
	private readonly string? currentQuery;
	private readonly int? currentPage;
	private readonly int? currentPerPage;
	private readonly IPackagesService<TPackage, TPackageVersion, TPackageList> service;

	public DefaultPackagesQueryBuilder(IPackagesService<TPackage, TPackageVersion, TPackageList> service)
	{
		this.service = service;
	}

	private DefaultPackagesQueryBuilder(IPackagesService<TPackage, TPackageVersion, TPackageList> service,
		string? author, string? query, int? page, int? perPage)
	{
		this.service = service;

		currentAuthor = author;
		currentQuery = query;
		currentPage = page;
		currentPerPage = perPage;
	}

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> ByAuthor(string author)
	{
		Console.WriteLine($"Author was: {currentAuthor ?? "(null)"}, now: {author}");

		return new DefaultPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>(service, author, currentQuery,
			currentPage, currentPerPage);
	}

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithQuery(string? query)
	{
		return new DefaultPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>(service, currentAuthor,
			query, currentPage, currentPerPage);
	}

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithPage(int? page)
	{
		return new DefaultPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>(service, currentAuthor,
			currentQuery, page, currentPerPage);
	}

	public IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> WithPerPage(int? perPage)
	{
		return new DefaultPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>(service, currentAuthor,
			currentQuery, currentPage, perPage);
	}

	public async Task<TPackageList> RunAsync(CancellationToken cancellationToken = default)
	{
		if (currentAuthor is not null)
			return await service.ByAuthorAsync(currentAuthor, currentQuery, currentPage, currentPerPage,
				cancellationToken);

		return await service.GetPackagesAsync(currentQuery, currentPage, currentPerPage, cancellationToken);
	}
}
