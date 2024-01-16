using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPackageVersionConfiguration, DartPackageVersion>]
public class DartPackageVersion
{
	public Guid Id { get; init; }

	public Guid PackageId { get; init; }

	public DartPackage Package { get; init; } = null!;

	public string Version { get; init; } = null!;

	public bool Retracted { get; init; }

	public DateTimeOffset PublishedAt { get; init; }
}
