using PubNet.Database.Entities.Nuget;

namespace PubNet.API.DTO.Packages.Nuget;

public class NugetPackageDto : PackageDto<NugetPackageVersionDto>
{
	public static NugetPackageDto MapFrom(NugetPackage package, bool includeVersions = true)
	{
		var latestVersion = package.LatestVersion is { } latest
			? NugetPackageVersionDto.MapFrom(latest)
			: null;

		var versions = includeVersions
			? package.Versions.Select(NugetPackageVersionDto.MapFrom).ToList()
			: null;

		return new()
		{
			Name = package.Name,
			Latest = latestVersion,
			Versions = versions,
			Type = PackageType.Nuget,
		};
	}
}
