using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PubNet.Database.Entities.Dart;

[NotMapped]
public class PubSpecScreenshot
{
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	[JsonPropertyName("path")]
	public string? Path { get; set; }
}
