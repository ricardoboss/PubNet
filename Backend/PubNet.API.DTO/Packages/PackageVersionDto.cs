namespace PubNet.API.DTO.Packages;

public abstract class PackageVersionDto
{
	public required string Version { get; init; }

	public required DateTimeOffset PublishedAt { get; init; }
}
