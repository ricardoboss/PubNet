using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetSearchResultDto
{
	[JsonPropertyName("totalHits")]
	public long TotalHits { get; init; }

	[JsonPropertyName("data")]
	public IEnumerable<NugetSearchResultHitDto> Data { get; init; } = [];
}
