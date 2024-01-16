using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class PackageVersionAnalysisDto
{
	[return: NotNullIfNotNull(nameof(analysis))]
	public static PackageVersionAnalysisDto? FromPackageVersionAnalysis(PackageVersionAnalysis? analysis, bool includeReadme = false)
	{
		if (analysis is null) return null;

		return new()
		{
			Formatted = analysis.Formatted,
			DocumentationHref = analysis.DocumentationLink,
			ReadmeFound = analysis.ReadmeFound,
			ReadmeText = includeReadme ? analysis.ReadmeText : null,
			CompletedAt = analysis.CompletedAtUtc,
		};
	}

	public bool? Formatted { get; init; }

	public string? DocumentationHref { get; init; }

	public bool? ReadmeFound { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string? ReadmeText { get; init; }

	public DateTimeOffset? CompletedAt { get; init; }
}
