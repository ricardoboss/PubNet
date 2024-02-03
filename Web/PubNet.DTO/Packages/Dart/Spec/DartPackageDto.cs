using PubNet.Database.Entities.Dart;

namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartPackageDto : PackageDto<DartPackageVersionDto>
{
	public static DartPackageDto MapFrom(DartPackage package, bool includeVersions = true)
	{
		var latestVersion = package.LatestVersion is { } latest
			? DartPackageVersionDto.MapFrom(latest)
			: null;

		var versions = includeVersions
			? package.Versions.Select(DartPackageVersionDto.MapFrom).ToList()
			: null;

		return new()
		{
			Name = package.Name,
			ReplacedBy = package.ReplacedBy,
			IsDiscontinued = package.IsDiscontinued,
			Latest = latestVersion,
			Versions = versions,
		};
	}

	public bool? IsDiscontinued { get; init; }

	public string? ReplacedBy { get; init; }
}
