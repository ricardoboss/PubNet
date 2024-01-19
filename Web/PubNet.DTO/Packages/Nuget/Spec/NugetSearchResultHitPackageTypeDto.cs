using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetSearchResultHitPackageTypeDto
{
	[JsonPropertyName("name")]
	public string Name { get; init; }
}
