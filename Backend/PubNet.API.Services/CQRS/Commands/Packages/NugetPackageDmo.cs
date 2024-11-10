using NuGet.Packaging;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Services.Extensions;
using PubNet.Database.Context;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Nuget;
using PubNet.PackageStorage.Abstractions;

namespace PubNet.API.Services.CQRS.Commands.Packages;

public class NugetPackageDmo(PubNetContext context, IArchiveStorage archiveStorage) : INugetPackageDmo
{
	public async Task<NugetPackageVersion> CreateAsync(Author author, byte[] nupkg,
		CancellationToken cancellationToken = default)
	{
		var package = new NugetPackage
		{
			Id = Guid.NewGuid(),
			Author = author,
			Versions = [],
		};

		var version = await ReadPackageVersion(package, nupkg, cancellationToken);

		package.Name = version.NuspecId!;
		package.Versions.Add(version);
		package.LatestVersion = version;
		package.LatestVersionId = version.Id;

		await context.NugetPackageVersions.AddAsync(version, cancellationToken);
		await context.NugetPackages.AddAsync(package, cancellationToken);

		version.Package = package;

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(package).ReloadAsync(cancellationToken);
		await context.Entry(version).ReloadAsync(cancellationToken);

		return version;
	}

	public async Task<NugetPackageVersion> AddVersionAsync(NugetPackage package, byte[] nupkg,
		CancellationToken cancellationToken = default)
	{
		var version = await ReadPackageVersion(package, nupkg, cancellationToken);

		await context.NugetPackageVersions.AddAsync(version, cancellationToken);

		package.Versions.Add(version);
		package.LatestVersion = version;

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(package).ReloadAsync(cancellationToken);
		await context.Entry(version).ReloadAsync(cancellationToken);

		return version;
	}

	private async Task<NugetPackageVersion> ReadPackageVersion(NugetPackage package, byte[] nupkg,
		CancellationToken cancellationToken)
	{
		using var packageStream = new MemoryStream(nupkg);

		using var reader = new PackageArchiveReader(packageStream);
		var nuspec = reader.NuspecReader;

		var id = nuspec.GetId().ToNullIfEmpty() ??
			throw new InvalidOperationException("Package ID is missing or empty");
		var version = nuspec.GetVersion();
		var title = nuspec.GetTitle().ToNullIfEmpty();
		var description = nuspec.GetDescription().ToNullIfEmpty();
		var authors = nuspec.GetAuthors().ToNullIfEmpty();
		var iconUrl = nuspec.GetIconUrl().ToNullIfEmpty();
		var projectUrl = nuspec.GetProjectUrl().ToNullIfEmpty();
		var copyright = nuspec.GetCopyright().ToNullIfEmpty();
		var tags = nuspec.GetTags().ToNullIfEmpty();
		var readme = nuspec.GetReadme().ToNullIfEmpty();
		var icon = nuspec.GetIcon().ToNullIfEmpty();
		var repositoryMetadata = nuspec.GetRepositoryMetadata();
		var dependencyGroups = nuspec.GetDependencyGroups() ?? [];

		var versionEntity = new NugetPackageVersion
		{
			Id = Guid.NewGuid(),
			Version = version.ToString(),
			PublishedAt = DateTimeOffset.UtcNow,
			NuspecId = id,
			NuspecVersion = version,
			Title = title,
			Description = description,
			Authors = authors,
			IconUrl = iconUrl,
			ProjectUrl = projectUrl,
			Copyright = copyright,
			Tags = tags,
			ReadmeFile = readme,
			IconFile = icon,
			RepositoryMetadata = repositoryMetadata,
			DependencyGroups = dependencyGroups.ToArray(),
		};

		_ = await archiveStorage.StoreArchiveAsync(package.Author.UserName, id, version.ToString(), nupkg,
			cancellationToken);

		return versionEntity;
	}
}
