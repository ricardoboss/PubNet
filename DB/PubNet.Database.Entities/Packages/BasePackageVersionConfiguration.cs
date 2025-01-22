using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Packages;

public abstract class BasePackageVersionConfiguration<[DynamicallyAccessedMembers(AotHelper.DynamicallyAccessedMemberTypes)] TVersion, TPackage> : IEntityTypeConfiguration<TVersion> where TVersion : BasePackageVersion<TPackage> where TPackage : BasePackage<TVersion>
{
	public virtual void Configure(EntityTypeBuilder<TVersion> builder)
	{
		builder.HasKey(v => v.Id);

		builder.HasIndex(v => new { v.PackageId, v.Version })
			.IsUnique();

		builder.HasIndex(v => v.PublishedAt)
			.IsDescending();
	}
}
