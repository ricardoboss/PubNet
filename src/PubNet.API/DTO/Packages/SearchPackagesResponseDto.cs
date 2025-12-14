using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages;

[PublicAPI]
public record SearchPackagesResponseDto(IEnumerable<SearchResultPackageDto> Packages);
