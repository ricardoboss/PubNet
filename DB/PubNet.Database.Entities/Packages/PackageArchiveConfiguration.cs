using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubNet.Database.Entities.Dart;
using PubNet.Database.Entities.Nuget;

namespace PubNet.Database.Entities.Packages;

public class PackageArchiveConfiguration : IEntityTypeConfiguration<PackageArchive>
{
	public void Configure(EntityTypeBuilder<PackageArchive> builder)
	{
		builder.ToTable("PackageArchives");

		builder.HasKey(a => a.Id);

		builder.HasIndex(a => new { a.PackageVersionId, a.PackageTypeDiscriminator })
			.IsUnique();

		builder.HasDiscriminator(a => a.PackageTypeDiscriminator)
			.HasValue<DartPackageVersionArchive>("dart")
			.HasValue<NugetPackageVersionArchive>("nuget");
	}
}
