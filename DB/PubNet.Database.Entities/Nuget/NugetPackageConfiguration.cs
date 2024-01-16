using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Nuget;

public class NugetPackageConfiguration : IEntityTypeConfiguration<NugetPackage>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<NugetPackage> builder)
	{
		builder.HasKey(p => p.Id);

		builder.HasIndex(p => new { p.AuthorId, p.Name })
			.IsUnique();

		builder.HasOne<Author>(p => p.Author)
			.WithMany(a => a.NugetPackages)
			.HasForeignKey(p => p.AuthorId);

		builder.HasMany<NugetPackageVersion>(p => p.Versions)
			.WithOne(v => v.Package)
			.HasForeignKey(v => v.PackageId);

		builder.HasOne<NugetPackageVersion>(p => p.LatestVersion)
			.WithOne()
			.HasPrincipalKey<NugetPackage>(p => p.LatestVersionId);

		builder.Property(p => p.Name)
			.HasMaxLength(50);

		builder.Navigation(p => p.LatestVersion)
			.AutoInclude();

		// TODO: enable lazy loading for author
	}
}
