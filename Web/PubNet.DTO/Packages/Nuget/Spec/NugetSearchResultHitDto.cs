using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetSearchResultHitDto
{
	[JsonPropertyName("@id")]
	public string Id { get; init; }

	[JsonPropertyName("@type")]
	public string Type { get; init; }

	[JsonPropertyName("registration")]
	public string Registration { get; init; }

	[JsonPropertyName("id")]
	public string PackageId { get; init; }

	[JsonPropertyName("version")]
	public string Version { get; init; }

	[JsonPropertyName("description")]
	public string Description { get; init; }

	[JsonPropertyName("summary")]
	public string Summary { get; init; }

	[JsonPropertyName("title")]
	public string Title { get; init; }

	[JsonPropertyName("licenseUrl")]
	public string LicenseUrl { get; init; }

	[JsonPropertyName("projectUrl")]
	public string ProjectUrl { get; init; }

	[JsonPropertyName("tags")]
	public string[] Tags { get; init; }

	[JsonPropertyName("authors")]
	public string[] Authors { get; init; }

	[JsonPropertyName("owners")]
	public string[] Owners { get; init; }

	[JsonPropertyName("totalDownloads")]
	public int TotalDownloads { get; init; }

	[JsonPropertyName("verified")]
	public bool Verified { get; init; }

	[JsonPropertyName("packageTypes")]
	public NugetSearchResultHitPackageTypeDto[] PackageTypes { get; init; }

	[JsonPropertyName("versions")]
	public NugetSearchResultHitVersionInfoDto[] Versions { get; init; }

	[JsonPropertyName("vulnerabilities")]
	public string[] Vulnerabilities { get; init; }
}
