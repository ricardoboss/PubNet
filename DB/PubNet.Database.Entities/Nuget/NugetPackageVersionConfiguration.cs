using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubNet.Database.Entities.Converters;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Nuget;

public class NugetPackageVersionConfiguration : BasePackageVersionConfiguration<NugetPackageVersion, NugetPackage>
{
	public override void Configure(EntityTypeBuilder<NugetPackageVersion> builder)
	{
		base.Configure(builder);

		builder.Property(v => v.DependencyGroups)
			.HasConversion<PackageDependencyGroupValueArrayConverter>()
			.HasColumnType("json");

		builder.Property(v => v.NuspecVersion)
			.HasConversion<NuGetVersionValueConverter>()
			.HasColumnType("text");

		builder.Property(v => v.RepositoryMetadata)
			.HasConversion<RepositoryMetadataValueConverter>()
			.HasColumnType("json");
	}
}
