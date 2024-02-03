namespace PubNet.API.DTO.Packages;

public class PackageListDto : PackageListDto<PackageDto<PackageVersionDto>, PackageVersionDto>;

public class PackageListDto<TPackageDto, TPackageVersionDto> where TPackageDto : PackageDto<TPackageVersionDto>
	where TPackageVersionDto : PackageVersionDto
{
	public required int TotalHits { get; init; }

	public required IEnumerable<TPackageDto> Packages { get; init; }
}
