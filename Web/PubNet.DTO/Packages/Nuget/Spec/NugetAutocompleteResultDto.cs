using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetAutocompleteResultDto
{
	[JsonPropertyName("totalHits")]
	public long TotalHits { get; init; }

	[JsonPropertyName("data")]
	public IEnumerable<string> Data { get; init; } = new List<string>();
}
