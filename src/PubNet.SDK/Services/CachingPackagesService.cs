using Microsoft.Extensions.Logging;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class CachingPackagesService(IPackagesService inner, ILogger<CachingPackagesService> logger) : IPackagesService
{
	private readonly Dictionary<(string name, bool includeAuthor), PackageDto> packagesCache = new();

	private readonly Dictionary<string, Dictionary<string, PackageVersionDto>>
		packageVersionsCache = new();

	private readonly Dictionary<string, AuthorPackagesResponseDto> authorPackagesCache = new();

	public async Task<PackageDto?> GetPackageAsync(string name, bool includeAuthor,
		CancellationToken cancellationToken = default)
	{
		if (packagesCache.TryGetValue((name, includeAuthor), out var package))
		{
			logger.LogTrace("Providing cached package {{ name={PackageName}, includeAuthor={IncludeAuthor} }}", name,
				includeAuthor);

			return package;
		}

		logger.LogTrace("Cache miss for package {{ name={PackageName}, includeAuthor={IncludeAuthor} }}", name,
			includeAuthor);

		package = await inner.GetPackageAsync(name, includeAuthor, cancellationToken);

		if (package is not null)
		{
			logger.LogTrace("Caching package {{ name={PackageName}, includeAuthor={IncludeAuthor} }}", name,
				includeAuthor);

			packagesCache[(name, includeAuthor)] = package;
		}

		return package;
	}

	private void PurgeCache(string name, string? version)
	{
		logger.LogTrace("Purging cache for {{ name={PackageName}, version={PackageVersion} }}", name, version);

		packagesCache.Remove((name, true));
		packagesCache.Remove((name, false));

		if (version is null)
			return;

		packageVersionsCache.Remove(name);
	}

	public async Task DeletePackageAsync(string name, CancellationToken cancellationToken = default)
	{
		await inner.DeletePackageAsync(name, cancellationToken);

		PurgeCache(name, null);
	}

	public async Task<PackageVersionDto?> GetPackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		if (packageVersionsCache.TryGetValue(name, out var packageVersions) &&
			packageVersions.TryGetValue(version, out var packageVersion))
		{
			logger.LogTrace("Providing cached package version {{ name={PackageName}, version={PackageVersion} }}", name,
				version);

			return packageVersion;
		}

		logger.LogTrace("Cache miss for package version {{ name={PackageName}, version={PackageVersion} }}", name,
			version);

		packageVersion = await inner.GetPackageVersionAsync(name, version, cancellationToken);

		if (packageVersion is not null)
		{
			logger.LogTrace("Caching package version {{ name={PackageName}, version={PackageVersion} }}", name, version);

			if (!packageVersionsCache.TryGetValue(name, out packageVersions))
			{
				packageVersions = new();
				packageVersionsCache[name] = packageVersions;
			}

			packageVersions[version] = packageVersion;
		}

		return packageVersion;
	}

	public async Task DeletePackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		await inner.DeletePackageVersionAsync(name, version, cancellationToken);

		PurgeCache(name, version);
	}

	public async Task DiscontinuePackageAsync(string name, string? replacement,
		CancellationToken cancellationToken = default)
	{
		await inner.DiscontinuePackageAsync(name, replacement, cancellationToken);

		PurgeCache(name, null);
	}

	public async Task RetractPackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		await inner.RetractPackageVersionAsync(name, version, cancellationToken);

		PurgeCache(name, version);
	}

	public async Task<AuthorPackagesResponseDto?> GetPackagesByAuthorAsync(string username,
		CancellationToken cancellationToken = default)
	{
		if (authorPackagesCache.TryGetValue(username, out var response))
		{
			logger.LogTrace("Providing cached author packages {{ username={AuthorUsername} }}", username);

			return response;
		}

		logger.LogTrace("Cache miss for author packages {{ username={AuthorUsername} }}", username);

		response = await inner.GetPackagesByAuthorAsync(username, cancellationToken);

		if (response is not null)
		{
			logger.LogTrace("Caching author packages {{ username={AuthorUsername} }}", username);

			authorPackagesCache[username] = response;
		}

		return response;
	}

	public async Task<SearchPackagesResponseDto?> SearchPackagesAsync(CancellationToken cancellationToken = default)
	{
		return await inner.SearchPackagesAsync(cancellationToken);
	}
}
