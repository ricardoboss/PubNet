using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Packages;

public abstract class BasePackageConfiguration<TPackage, TVersion> : IEntityTypeConfiguration<TPackage> where TPackage : BasePackage<TVersion> where TVersion : BasePackageVersion<TPackage>
{
	public virtual void Configure(EntityTypeBuilder<TPackage> builder)
	{
		// TODO: split up into multiple methods

		builder.HasKey(p => p.Id);

		builder.HasIndex(p => new { p.AuthorId, p.Name })
			.IsUnique();

		builder.HasOne<Author>(p => p.Author)
			.WithMany(NavigateFromAuthorToPackages())
			.HasForeignKey(p => p.AuthorId);

		builder.HasMany<TVersion>(p => p.Versions)
			.WithOne(v => v.Package)
			.HasForeignKey(v => v.PackageId);

		builder.HasOne<TVersion>(p => p.LatestVersion)
			.WithOne()
			.HasPrincipalKey<TPackage>(p => p.LatestVersionId);

		builder.Property(p => p.Name)
			.HasMaxLength(50);

		builder.Navigation(p => p.LatestVersion)
			.AutoInclude();

		// TODO: enable lazy loading for author
	}

	protected abstract Expression<Func<Author, IEnumerable<TPackage>?>> NavigateFromAuthorToPackages();
}
