namespace PubNet.Database.Entities.Packages;

public class PackageArchive
{
	public Guid Id { get; init; }

	public Guid PackageVersionId { get; init; }

	public string PackageTypeDiscriminator { get; init; } = null!;

	public string ArchiveUrl { get; init; } = null!;

	public string ArchiveType { get; init; } = null!;

	public string ArchiveSha256 { get; init; } = null!;
}
