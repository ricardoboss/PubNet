using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class PackageVersionDto
{
	public string Version { get; init; }
	public string PackageName { get; init; }
	public bool Retracted { get; init; }

	[JsonPropertyName("archive_url")] public string ArchiveUrl { get; init; }

	[JsonPropertyName("archive_sha256")] public string ArchiveSha256 { get; init; }

	public DateTimeOffset Published { get; init; }
	public PubSpec PubSpec { get; init; }
	public PackageVersionAnalysisDto? Analysis { get; init; }

	public bool? Mirrored { get; set; }

	public static PackageVersionDto FromPackageVersion(PackageVersion version)
	{
		return new()
		{
			Version = version.Version,
			PackageName = version.PackageName,
			Retracted = version.Retracted,
			ArchiveUrl = version.ArchiveUrl,
			ArchiveSha256 = version.ArchiveSha256,
			Published = version.PublishedAtUtc,
			PubSpec = version.PubSpec,
			Analysis = PackageVersionAnalysisDto.FromPackageVersionAnalysis(version.Analysis),
			Mirrored = false,
		};
	}
}
