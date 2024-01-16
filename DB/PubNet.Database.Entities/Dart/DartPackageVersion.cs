using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPackageVersionConfiguration, DartPackageVersion>]
public class DartPackageVersion : BasePackageVersion<DartPackage>
{
	public bool Retracted { get; init; }

	public PubSpec PubSpec { get; init; } = null!;
}
