using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Dart;

public class DartPendingArchiveConfiguration : IEntityTypeConfiguration<DartPendingArchive>
{
	public void Configure(EntityTypeBuilder<DartPendingArchive> builder)
	{
		builder.HasKey(a => a.Id);

		builder.Property(a => a.ArchivePath)
			.HasMaxLength(250);

		builder.HasOne<Author>(a => a.Uploader)
			.WithMany()
			.HasForeignKey(a => a.UploaderId);
	}
}
