using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubNet.Database.Entities.Converters;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Dart;

public class DartPackageVersionConfiguration : BasePackageVersionConfiguration<DartPackageVersion, DartPackage>
{
	public override void Configure(EntityTypeBuilder<DartPackageVersion> builder)
	{
		base.Configure(builder);

		builder.Property(v => v.PubSpec)
			.HasConversion(new PubSpecValueConverter())
			.HasColumnType("json");
	}
}
