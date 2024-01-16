using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class PackageVersionDto
{
	public string Version { get; init; } = null!;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Retracted { get; set; }

	[JsonPropertyName("archive_url")]
	public string ArchiveUrl { get; init; } = null!;

	[JsonPropertyName("archive_sha256")]
	public string ArchiveSha256 { get; init; } = null!;

	public DateTimeOffset Published { get; init; }

	[JsonPropertyName("pubspec")]
	public PubSpec PubSpec { get; init; } = null!;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Mirrored { get; set; }

	public static PackageVersionDto FromPackageVersion(DartPackageVersion version)
	{
		return new()
		{
			Version = version.Version,
			Retracted = version.Retracted,
			ArchiveUrl = version.ArchiveUrl,
			ArchiveSha256 = version.ArchiveSha256.ToLowerInvariant(),
			Published = version.PublishedAtUtc,
			PubSpec = version.PubSpec,
			Mirrored = false,
		};
	}
}
