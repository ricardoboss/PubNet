using PubNet.Database.Entities.Dart;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Packages.Dart;

[Mapper]
public partial class DartPackageVersionAnalysisDto
{
	[MapProperty([nameof(DartPackageVersionAnalysis.PackageVersion), nameof(DartPackageVersionAnalysis.PackageVersion.Version)], [nameof(PackageVersion)])]
	[MapProperty([nameof(DartPackageVersionAnalysis.PackageVersion), nameof(DartPackageVersionAnalysis.PackageVersion.Package), nameof(DartPackageVersionAnalysis.PackageVersion.Package.Name)], [nameof(PackageName)])]
	public static partial DartPackageVersionAnalysisDto MapFrom(DartPackageVersionAnalysis analysis);

	public string PackageName { get; set; } = null!;

	public string PackageVersion { get; set; } = null!;

	public bool? Formatted { get; set; }

	public bool? DocumentationGenerated { get; set; }

	public bool? ReadmeFound { get; set; }

	public string? ReadmeText { get; set; }

	public DateTimeOffset? CompletedAt { get; set; }
}
