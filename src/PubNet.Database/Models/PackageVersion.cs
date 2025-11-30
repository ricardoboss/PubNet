using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PubNet.Database.Models;

public class PackageVersion
{
	public int Id { get; set; }

	public string PackageName { get; set; } = string.Empty;

	public string Version { get; set; } = string.Empty;

	public bool Retracted { get; set; }

	public string ArchiveUrl { get; set; } = string.Empty;

	[MaxLength(64)]
	public string ArchiveSha256 { get; set; } = string.Empty;

	public DateTimeOffset PublishedAtUtc { get; set; }

	[NotNull] public PubSpec? PubSpec { set; get; }

	public int? AnalysisId { get; set; }

	public PackageVersionAnalysis? Analysis { get; set; }
}
