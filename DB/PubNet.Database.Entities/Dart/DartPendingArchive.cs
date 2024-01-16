using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPendingArchiveConfiguration, DartPendingArchive>]
public class DartPendingArchive
{
	public Guid Id { get; init; }

	public string ArchivePath { get; init; } = null!;

	public string UnpackedArchivePath => ArchivePath[..^".tar.gz".Length];

	public Guid UploaderId { get; init; }

	public Author Uploader { get; init; } = null!;

	public DateTimeOffset UploadedAt { get; init; }
}
