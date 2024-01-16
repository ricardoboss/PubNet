using System.Diagnostics.CodeAnalysis;

namespace PubNet.Database.Models;

public class DartPackageVersion
{
	public int Id { get; set; }

	public string PackageName { get; set; } = string.Empty;

	public string Version { get; set; } = string.Empty;

	public bool Retracted { get; set; }

	public string ArchiveUrl { get; set; } = string.Empty;

	public string ArchiveSha256 { get; set; } = string.Empty;

	public DateTimeOffset PublishedAtUtc { get; set; }

	public PubSpec? PubSpec { set; get; }

	public int? AnalysisId { get; set; }

	public PackageVersionAnalysis? Analysis { get; set; }
}
