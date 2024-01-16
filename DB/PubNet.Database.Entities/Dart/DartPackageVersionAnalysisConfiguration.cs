using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Dart;

public class DartPackageVersionAnalysisConfiguration : IEntityTypeConfiguration<DartPackageVersionAnalysis>
{
	public void Configure(EntityTypeBuilder<DartPackageVersionAnalysis> builder)
	{
		builder.HasKey(a => a.Id);

		builder.HasOne<DartPackageVersion>(a => a.PackageVersion)
			.WithOne()
			.HasPrincipalKey<DartPackageVersionAnalysis>(a => a.PackageVersionId);
	}
}
