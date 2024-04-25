﻿namespace PubNet.API.DTO.Packages;

public abstract class PackageDto : PackageDto<PackageVersionDto>;

public abstract class PackageDto<TVersionDto> where TVersionDto : PackageVersionDto
{

	public required string Name { get; init; }

	public required TVersionDto? Latest { get; init; }

	public required List<TVersionDto>? Versions { get; init; }
}