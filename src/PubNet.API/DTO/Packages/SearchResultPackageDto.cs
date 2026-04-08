using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages;

[PublicAPI]
public record SearchResultPackageDto(
	string Name,
	string? ReplacedBy,
	bool IsDiscontinued,
	string? AuthorUserName,
	string? LatestVersion,
	DateTimeOffset? LatestPublishedAtUtc
);
