using System.Text.Json.Serialization;

namespace PubNet.Database.Models;

public class PubSpecScreenshot
{
	[JsonPropertyName("desciption")] public string? Description { get; set; }

	[JsonPropertyName("path")] public string? Path { get; set; }
}
