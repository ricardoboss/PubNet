using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPackageVersionConfiguration, DartPackageVersion>]
public class DartPackageVersion : BasePackageVersion<DartPackage>
{
	public bool Retracted { get; set; }

	public PubSpec PubSpec { get; init; } = null!;

	public Guid? AnalysisId { get; init; }

	public DartPackageVersionAnalysis? Analysis { get; set; }
}
