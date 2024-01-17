using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Dart;

[EntityTypeConfiguration<DartPackageConfiguration, DartPackage>]
public class DartPackage : BasePackage<DartPackageVersion>
{
	public bool IsDiscontinued { get; set; }

	public string? ReplacedBy { get; set; }
}
