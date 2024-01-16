using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Dart;

public class DartPackageVersionArchive : PackageArchive
{
	public DartPackageVersion PackageVersion { get; init; } = null!;
}
