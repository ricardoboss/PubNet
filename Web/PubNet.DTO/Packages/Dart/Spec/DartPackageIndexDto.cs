using PubNet.Database.Entities.Dart;

namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartPackageIndexDto
{
	public static DartPackageIndexDto MapFrom(DartPackage package)
	{
		return new()
		{
			Name = package.Name,
			ReplacedBy = package.ReplacedBy,
			IsDiscontinued = package.IsDiscontinued,
			Latest = package.LatestVersion is {} latest ? DartPackageVersionDto.MapFrom(latest) : null,
			Versions = package.Versions.Select(DartPackageVersionDto.MapFrom).ToList(),
		};
	}

	public required string Name { get; init; }

	public bool? IsDiscontinued { get; init; }

	public string? ReplacedBy { get; init; }

	public required DartPackageVersionDto? Latest { get; init; }

	public required List<DartPackageVersionDto> Versions { get; init; }
}
