using PubNet.Database.Models;

namespace PubNet.API.DTO;

public record PackageVersionAnalysisDto(bool? Formatted, string? DocumentationHref, string? ReadmeText, DateTimeOffset? CompletedAt)
{
	public static PackageVersionAnalysisDto? FromPackageVersionAnalysis(PackageVersionAnalysis? analysis)
	{
		if (analysis is null) return null;

		return new(
			analysis.Formatted,
			analysis.DocumentationLink,
			analysis.ReadmeText,
			analysis.CompletedAtUtc
		);
	}
}
