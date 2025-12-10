using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages;

[PublicAPI]
public record SearchPackagesResponse(IEnumerable<SearchResultPackage> Packages);
