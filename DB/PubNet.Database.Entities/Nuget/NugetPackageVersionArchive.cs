using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Nuget;

public class NugetPackageVersionArchive : PackageArchive
{
	public virtual NugetPackageVersion PackageVersion { get; init; } = null!;
}
