using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Nuget;

public class NugetPackageVersionArchive : PackageArchive
{
	public NugetPackageVersion PackageVersion { get; init; } = null!;
}
