using DartLang.PubSpec;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartPackageVersionDto : PackageVersionDto
{
	public static DartPackageVersionDto MapFrom(DartPackageVersion version, Uri archiveUrl, string archiveSha256)
	{
		return new DartPackageVersionDto
		{
			Version = version.Version,
			ArchiveUrl = archiveUrl,
			ArchiveSha256 = archiveSha256,
			Retracted = version.Retracted,
			Pubspec = version.PubSpec,
			PublishedAt = version.PublishedAt,
		};
	}

	public bool? Retracted { get; init; }

	/// <summary>
	/// The <c>archive_url</c> may be temporary and is allowed to include query-string parameters. This allows for the
	/// server to return signed URLs for S3, GCS, or other blob storage services. If temporary URLs are returned it is
	/// wise to not set expiration to less than 25 minutes (to allow for retries and clock drift).
	/// </summary>
	public required Uri ArchiveUrl { get; init; }

	/// <summary>
	/// <para>
	/// The <c>archive_sha256</c> should be the hex-encoded sha256 checksum of the file at archive_url. It is an
	/// optional field that allows the pub client to verify the integrity of the downloaded archive.
	/// </para>
	/// <para>
	/// The <c>archive_sha256</c> also provides an easy way for clients to detect if something has changed on the
	/// server. In the absence of this field, the client can still download the archive to obtain a checksum and detect
	/// changes to the archive.
	/// </para>
	/// </summary>
	public required string ArchiveSha256 { get; init; }

	public required PubSpec Pubspec { get; init; }
}
