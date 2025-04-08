using DartLang.PubSpec;
using Microsoft.Extensions.Logging;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.Database.Context;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Dart;
using Semver;

namespace PubNet.API.Services.CQRS.Commands.Packages;

public class DartPackageDmo(PubNet2Context context, ILogger<DartPackageDmo> logger) : IDartPackageDmo
{
	public async Task<DartPackageVersion> CreateAsync(string name, Author author, SemVersion initialVersion, PubSpec pubSpec, CancellationToken cancellationToken = default)
	{
		logger.LogTrace("Creating package {PackageName} for author {Author} with initial version {Version}", name, author.UserName, initialVersion);

		var version = new DartPackageVersion
		{
			Id = Guid.NewGuid(),
			Version = initialVersion.ToString(),
			PublishedAt = DateTimeOffset.UtcNow,
			PubSpec = pubSpec,
		};

		await context.DartPackageVersions.AddAsync(version, cancellationToken);

		var package = new DartPackage
		{
			Name = name,
			Author = author,
			Versions = [version],
			LatestVersionId = version.Id,
		};

		await context.DartPackages.AddAsync(package, cancellationToken);

		version.Package = package;

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(package).ReloadAsync(cancellationToken);
		await context.Entry(version).ReloadAsync(cancellationToken);

		return version;
	}

	public Task RetractAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task DiscontinueAsync(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task SaveLatestVersionAsync(DartPackage package, DartPackageVersion version,
		CancellationToken cancellationToken = default)
	{
		logger.LogTrace("Saving latest version {Version} for package {PackageName}", version.Version, package.Name);

		package.Versions.Add(version);
		package.LatestVersion = version;

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(package).ReloadAsync(cancellationToken);
		await context.Entry(version).ReloadAsync(cancellationToken);
	}
}
