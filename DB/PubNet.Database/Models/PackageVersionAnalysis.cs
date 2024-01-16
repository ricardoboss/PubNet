using System.Diagnostics.CodeAnalysis;

namespace PubNet.Database.Models;

public class PackageVersionAnalysis
{
	public int Id { get; set; }

	public int VersionId { get; set; }

	[NotNull] public DartPackageVersion? Version { get; set; }

	public bool? Formatted { get; set; }

	public string? DocumentationLink { get; set; }

	public bool? ReadmeFound { get; set; }

	public string? ReadmeText { get; set; }

	public DateTimeOffset? CompletedAtUtc { get; set; }

	public bool IsComplete()
	{
		return Formatted is not null && DocumentationLink is not null && ReadmeFound is not null;
	}
}
