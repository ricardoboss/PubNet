using PubNet.Database.Entities.Dart;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Packages.Dart;

[Mapper]
public partial class DartPackageVersionAnalysisDto
{
	public static partial DartPackageVersionAnalysisDto MapFrom(DartPackageVersionAnalysis analysis);

	public bool? Formatted { get; init; }

	public bool? DocumentationGenerated { get; init; }

	public string? ReadmeText { get; init; }

	public DateTimeOffset? CompletedAt { get; init; }
}
