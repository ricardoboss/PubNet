using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPackageVersionAnalysisConfiguration, DartPackageVersionAnalysis>]
public class DartPackageVersionAnalysis
{
	public Guid Id { get; init; }

	public Guid PackageVersionId { get; init; }

	public virtual DartPackageVersion PackageVersion { get; init; } = null!;

	public bool? Formatted { get; set; }

	public bool? DocumentationGenerated { get; set; }

	public bool? ReadmeFound { get; set; }

	public string? ReadmeText { get; set; }

	public string? SpdxLicenseIdentifier { get; set; }

	public DateTimeOffset? CompletedAt { get; set; }

	public bool IsComplete()
	{
		return Formatted is not null && DocumentationGenerated is not null && ReadmeFound is not null &&
			SpdxLicenseIdentifier is not null;
	}
}
