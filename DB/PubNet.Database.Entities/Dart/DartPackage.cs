using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPackageConfiguration, DartPackage>]
public class DartPackage
{
	public Guid Id { get; init; }

	public Guid AuthorId { get; init; }

	public Author Author { get; init; } = null!;

	public string Name { get; init; } = null!;

	public bool IsDiscontinued { get; init; }

	public string? ReplacedBy { get; init; }

	public Guid? LatestVersionId { get; init; }

	public DartPackageVersion? LatestVersion { get; init; }

	public ICollection<DartPackageVersion> Versions { get; init; } = null!;
}
