using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetPackageIndexDto
{
	[JsonPropertyName("versions")]
	public IEnumerable<string> Versions { get; init; } = new List<string>();
}
