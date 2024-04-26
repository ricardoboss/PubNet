using PubNet.Database.Entities.Dart;

namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartPackageDto : PackageDto<DartPackageVersionDto>
{
	public static DartPackageDto MapFrom(DartPackage package, Func<string, string, (Uri url, string sha256)> archiveProvider, bool includeVersions = true)
	{
		DartPackageVersionDto? latestVersion = null;

		if (package.LatestVersion is { } latest)
		{
			var (latestArchiveUrl, latestArchiveSha256) = archiveProvider(package.Name, package.LatestVersion!.Version);
			latestVersion = DartPackageVersionDto.MapFrom(latest, latestArchiveUrl, latestArchiveSha256);
		}

		var versions = includeVersions
			? package.Versions.Select(v =>
			{
				var (archiveUrl, archiveSha256) = archiveProvider(package.Name, v.Version);

				return DartPackageVersionDto.MapFrom(v, archiveUrl, archiveSha256);
			}).ToList()
			: null;

		return new()
		{
			Name = package.Name,
			ReplacedBy = package.ReplacedBy,
			IsDiscontinued = package.IsDiscontinued,
			Latest = latestVersion,
			Versions = versions,
			Type = PackageType.Dart,
		};
	}

	public bool? IsDiscontinued { get; init; }

	public string? ReplacedBy { get; init; }
}
