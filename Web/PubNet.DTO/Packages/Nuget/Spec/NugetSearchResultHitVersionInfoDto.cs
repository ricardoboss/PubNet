using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetSearchResultHitVersionInfoDto
{
	/// <summary>
	/// The absolute URL to the associated registration
	/// <a href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-leaf">leaf</a>.
	/// </summary>
	[JsonPropertyName("@id")]
	public required string Id { get; init; }

	/// <summary>
	/// The full SemVer 2.0.0 version string of the package (could contain build metadata).
	/// </summary>
	public required string Version { get; init; }

	/// <summary>
	/// The number of downloads for this specific package version.
	/// </summary>
	public required int Downloads { get; init; }
}
