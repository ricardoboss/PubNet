namespace PubNet.API.DTO;

public record SearchResultPackage(string Name, string? ReplacedBy, bool IsDiscontinued, string? AuthorUserName, string LatestVersion, DateTimeOffset LatestPublishedAtUtc);