using PubNet.Database.Entities.Nuget;

namespace PubNet.API.DTO.Packages.Nuget;

public class NugetPackageVersionDto : PackageVersionDto
{
	public static NugetPackageVersionDto MapFrom(NugetPackageVersion version)
	{
		return new()
		{
			Version = version.Version,
			PublishedAt = version.PublishedAt,
		};
	}
}
