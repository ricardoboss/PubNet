using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Nuget;

[EntityTypeConfiguration<NugetPackageVersionConfiguration, NugetPackageVersion>]
public class NugetPackageVersion : BasePackageVersion<NugetPackage>
{
	public string? NuspecId { get; init; }

	public NuGetVersion? NuspecVersion { get; init; }

	public string? Title { get; init; }

	public string? Description { get; init; }

	public string? Authors { get; init; }

	public string? IconUrl { get; init; }

	public string? ProjectUrl { get; init; }

	public string? Copyright { get; init; }

	public string? Tags { get; init; }

	public RepositoryMetadata? RepositoryMetadata { get; init; }

	public string? ReadmeFile { get; init; }

	public string? IconFile { get; init; }

	public PackageDependencyGroup[]? DependencyGroups { get; init; }
}
