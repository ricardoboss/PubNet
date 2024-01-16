using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Nuget;

[EntityTypeConfiguration<NugetPackageConfiguration, NugetPackage>]
public class NugetPackage
{
	public Guid Id { get; init; }

	public Guid AuthorId { get; init; }

	public Author Author { get; init; } = null!;

	public string Name { get; init; } = null!;

	public Guid? LatestVersionId { get; init; }

	public NugetPackageVersion? LatestVersion { get; init; }

	public ICollection<NugetPackageVersion> Versions { get; init; } = null!;
}
