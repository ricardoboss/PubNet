using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPackageVersionAnalysisConfiguration, DartPackageVersionAnalysis>]
public class DartPackageVersionAnalysis
{
	public Guid Id { get; init; }

	public Guid PackageVersionId { get; init; }

	public DartPackageVersion PackageVersion { get; init; } = null!;

	public bool? Formatted { get; init; }

	public string? DocumentationLink { get; init; }

	public bool? ReadmeFound { get; init; }

	public string? ReadmeText { get; init; }

	public DateTimeOffset? CompletedAt { get; init; }

	public bool IsComplete()
	{
		return Formatted is not null && DocumentationLink is not null && ReadmeFound is not null;
	}
}
