using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Dart;

public class DartPackageConfiguration : BasePackageConfiguration<DartPackage, DartPackageVersion>
{
	/// <inheritdoc />
	public override void Configure(EntityTypeBuilder<DartPackage> builder)
	{
		base.Configure(builder);

		builder.Property(p => p.ReplacedBy)
			.HasMaxLength(50);
	}

	protected override Expression<Func<Author, IEnumerable<DartPackage>?>> NavigateFromAuthorToPackages()
	{
		return a => a.DartPackages;
	}
}
