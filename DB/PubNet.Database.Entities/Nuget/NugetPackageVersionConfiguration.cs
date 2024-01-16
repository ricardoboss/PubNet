using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Nuget;

public class NugetPackageVersionConfiguration : IEntityTypeConfiguration<NugetPackageVersion>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<NugetPackageVersion> builder)
	{
		builder.HasKey(v => v.Id);

		builder.HasIndex(v => new { v.PackageId, v.Version })
			.IsUnique();

		builder.HasIndex(v => v.PublishedAt)
			.IsDescending();
	}
}
