using JetBrains.Annotations;
using PubNet.API.DTO.Packages;

namespace PubNet.API.DTO.Authors;

[PublicAPI]
public record AuthorPackagesResponseDto(IEnumerable<PackageDto> Packages);
