using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Nuget;

[EntityTypeConfiguration<NugetPackageVersionConfiguration, NugetPackageVersion>]
public class NugetPackageVersion
{
	public Guid Id { get; init; }

	public Guid PackageId { get; init; }

	public NugetPackage Package { get; init; } = null!;

	public string Version { get; init; } = null!;

	public DateTimeOffset PublishedAt { get; init; }
}
