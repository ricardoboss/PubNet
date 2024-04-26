using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPendingArchiveConfiguration, DartPendingArchive>]
public class DartPendingArchive
{
	public Guid Id { get; init; }

	public string ArchivePath { get; init; } = null!;

	public string ArchiveHash { get; init; } = null!;

	public Guid UploaderId { get; init; }

	public virtual Author Uploader { get; init; } = null!;

	public DateTimeOffset UploadedAt { get; init; }
}
