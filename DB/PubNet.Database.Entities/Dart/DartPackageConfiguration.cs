using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Dart;

public class DartPackageConfiguration : IEntityTypeConfiguration<DartPackage>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<DartPackage> builder)
	{
		builder.HasKey(p => p.Id);

		builder.HasIndex(p => new { p.AuthorId, p.Name })
			.IsUnique();

		builder.HasOne<Author>(p => p.Author)
			.WithMany(a => a.DartPackages)
			.HasForeignKey(p => p.AuthorId);

		builder.HasMany<DartPackageVersion>(p => p.Versions)
			.WithOne(v => v.Package)
			.HasForeignKey(v => v.PackageId);

		builder.HasOne<DartPackageVersion>(p => p.LatestVersion)
			.WithOne()
			.HasPrincipalKey<DartPackage>(p => p.LatestVersionId);

		builder.Property(p => p.Name)
			.HasMaxLength(50);

		builder.Property(p => p.ReplacedBy)
			.HasMaxLength(50);

		builder.Navigation(p => p.LatestVersion)
			.AutoInclude();

		// TODO: enable lazy loading for author
	}
}
