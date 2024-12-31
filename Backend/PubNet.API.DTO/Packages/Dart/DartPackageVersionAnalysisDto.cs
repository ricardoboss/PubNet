using PubNet.Database.Entities.Dart;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Packages.Dart;

[Mapper]
public partial class DartPackageVersionAnalysisDto
{
	[MapperIgnoreSource(nameof(DartPackageVersionAnalysis.Id))]
	[MapperIgnoreSource(nameof(DartPackageVersionAnalysis.PackageVersionId))]
	[MapperIgnoreSource(nameof(DartPackageVersionAnalysis.PackageVersion))]
	[MapperIgnoreSource(nameof(DartPackageVersionAnalysis.ReadmeFound))]
	public static partial DartPackageVersionAnalysisDto MapFrom(DartPackageVersionAnalysis analysis);

	public bool? Formatted { get; init; }

	public bool? DocumentationGenerated { get; init; }

	public string? ReadmeText { get; init; }

	public string? SpdxLicenseIdentifier { get; init; }

	public DateTimeOffset? CompletedAt { get; init; }
}
