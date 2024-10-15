using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Dart;

public class DartPackageVersionArchive : PackageArchive
{
	public virtual DartPackageVersion PackageVersion { get; init; } = null!;
}
