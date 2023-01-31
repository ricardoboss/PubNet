namespace PubNet.API.DTO;

public record SearchPackagesResponse(IEnumerable<SearchResultPackage> Packages);