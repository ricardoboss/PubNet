using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// Each registration leaf object represents data associated with a single package version.
/// </summary>
public class NugetPackageRegistrationLeafDto
{
	/// <summary>
	/// The URL to the registration leaf.
	/// </summary>
	[JsonPropertyName("@id")]
	public required string Id { get; init; }

	/// <summary>
	/// The catalog entry containing the package metadata.
	/// </summary>
	[JsonPropertyName("catalogEntry")]
	public required NugetCatalogEntryDto CatalogEntry { get; init; }

	/// <summary>
	/// The URL to the package content (.nupkg).
	/// </summary>
	[JsonPropertyName("packageContent")]
	public required string PackageContent { get; init; }
}
