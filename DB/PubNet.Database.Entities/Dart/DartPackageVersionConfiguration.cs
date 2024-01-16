using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Dart;

public class DartPackageVersionConfiguration : IEntityTypeConfiguration<DartPackageVersion>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<DartPackageVersion> builder)
	{
		builder.HasKey(v => v.Id);

		builder.HasIndex(v => new { v.PackageId, v.Version })
			.IsUnique();

		builder.HasIndex(v => v.PublishedAt)
			.IsDescending();
	}
}
