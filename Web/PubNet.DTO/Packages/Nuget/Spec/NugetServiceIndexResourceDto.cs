using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetServiceIndexResourceDto
{
	[JsonPropertyName("@id")]
	public string Id { get; init; }

	[JsonPropertyName("@type")]
	public string Type { get; init; }
}
