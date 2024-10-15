using PubNet.Database.Entities.Dart;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Packages.Dart;

[Mapper]
public partial class DartPackageVersionAnalysisDto
{
	[MapProperty([nameof(DartPackageVersionAnalysis.PackageVersion), nameof(DartPackageVersionAnalysis.PackageVersion.Version)], nameof(PackageVersion))]
	[MapProperty([nameof(DartPackageVersionAnalysis.PackageVersion), nameof(DartPackageVersionAnalysis.PackageVersion.Package), nameof(DartPackageVersionAnalysis.PackageVersion.Package.Name)], nameof(PackageName))]
	public static partial DartPackageVersionAnalysisDto MapFrom(DartPackageVersionAnalysis analysis);

	public required string PackageName { get; init; }

	public required string PackageVersion { get; init; }

	public bool? Formatted { get; init; }

	public bool? DocumentationGenerated { get; init; }

	public bool? ReadmeFound { get; init; }

	public string? ReadmeText { get; init; }

	public DateTimeOffset? CompletedAt { get; init; }
}
