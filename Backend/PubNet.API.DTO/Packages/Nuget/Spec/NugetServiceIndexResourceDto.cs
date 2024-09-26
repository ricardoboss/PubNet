using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetServiceIndexResourceDto
{
	[JsonPropertyName("@id")]
	public required string Id { get; init; }

	[JsonPropertyName("@type")]
	public required string Type { get; init; }

	public string? Comment { get; init; }
}
