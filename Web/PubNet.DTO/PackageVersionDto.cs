using System.Text.Json.Serialization;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.DTO;

public class PackageVersionDto
{
	public string Version { get; init; } = null!;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Retracted { get; set; }

	public DateTimeOffset Published { get; init; }

	[JsonPropertyName("pubspec")]
	public PubSpec PubSpec { get; init; } = null!;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Mirrored { get; set; }

	public static PackageVersionDto FromPackageVersion(DartPackageVersion version)
	{
		return new()
		{
			Version = version.Version,
			Retracted = version.Retracted,
			Published = version.PublishedAt,
			PubSpec = version.PubSpec,
			Mirrored = false,
		};
	}
}
