using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetSearchResultHitVersionInfoDto
{
	[JsonPropertyName("version")]
	public string Version { get; init; }

	[JsonPropertyName("downloads")]
	public int Downloads { get; init; }

	[JsonPropertyName("@id")]
	public string Id { get; init; }
}
