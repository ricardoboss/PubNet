using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class PackageVersionDto
{
	public string Version { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Retracted { get; init; }

	[JsonPropertyName("archive_url")]
	public string ArchiveUrl { get; init; }

	[JsonPropertyName("archive_sha256")]
	public string ArchiveSha256 { get; init; }


	public DateTimeOffset Published { get; init; }

	[JsonPropertyName("pubspec")]
	public PubSpec PubSpec { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public PackageVersionAnalysisDto? Analysis { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Mirrored { get; set; }

	public static PackageVersionDto FromPackageVersion(PackageVersion version)
	{
		return new()
		{
			Version = version.Version,
			Retracted = version.Retracted,
			ArchiveUrl = version.ArchiveUrl,
			ArchiveSha256 = version.ArchiveSha256.ToLowerInvariant(),
			Published = version.PublishedAtUtc,
			PubSpec = version.PubSpec,
			Analysis = PackageVersionAnalysisDto.FromPackageVersionAnalysis(version.Analysis),
			Mirrored = false,
		};
	}
}
