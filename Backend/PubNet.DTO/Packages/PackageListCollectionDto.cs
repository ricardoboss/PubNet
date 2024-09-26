using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Nuget;

namespace PubNet.API.DTO.Packages;

public class PackageListCollectionDto
{
	public required DartPackageListDto Dart { get; init; }

	public required NugetPackageListDto Nuget { get; init; }
}
