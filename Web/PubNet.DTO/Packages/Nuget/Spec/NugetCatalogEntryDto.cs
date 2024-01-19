using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// <para>
/// The package <c>version</c> property is the full version string after normalization. This means that SemVer 2.0.0
/// build data can be included here.
/// </para>
/// <para>
/// The <c>dependencyGroups</c> property is an array of objects representing the dependencies of the package, grouped
/// by target framework. If the package has no dependencies, the <c>dependencyGroups</c> property is missing, an empty
/// array, or the <c>dependencies</c> property of all groups is empty or missing.
/// </para>
/// <para>
/// The value of the <c>licenseExpression</c> property complies with
/// <a href="https://learn.microsoft.com/en-us/nuget/reference/nuspec#license">NuGet license expression syntax</a>.
/// </para>
/// </summary>
public class NugetCatalogEntryDto
{
	/// <summary>
	/// The URL to the document used to produce this object.
	/// </summary>
	[JsonPropertyName("@id")]
	public required string Id { get; init; }

	[JsonPropertyName("authors")]
	public string? Authors { get; init; }

	/// <summary>
	/// The dependencies of the package, grouped by target framework.
	/// </summary>
	[JsonPropertyName("dependencyGroups")]
	public IEnumerable<NugetPackageDependencyGroupDto>? DependencyGroups { get; init; }

	/// <summary>
	/// The deprecation associated with the package.
	/// </summary>
	[JsonPropertyName("deprecation")]
	public NugetPackageDeprecationDto? Deprecation { get; init; }

	[JsonPropertyName("description")]
	public string? Description { get; init; }

	[JsonPropertyName("iconUrl")]
	public string? IconUrl { get; init; }

	/// <summary>
	/// The ID of the package.
	/// </summary>
	[JsonPropertyName("id")]
	public required string PackageId { get; init; }

	[JsonPropertyName("language")]
	public string? Language { get; init; }

	[JsonPropertyName("licenseUrl")]
	public string? LicenseUrl { get; init; }

	[JsonPropertyName("licenseExpression")]
	public string? LicenseExpression { get; init; }

	/// <summary>
	/// Should be considered as listed if absent.
	/// </summary>
	[JsonPropertyName("listed")]
	public bool? Listed { get; init; }

	[JsonPropertyName("minClientVersion")]
	public string? MinClientVersion { get; init; }

	/// <summary>
	/// Duplicate of the same property in the parent object, included only for legacy reasons.
	/// </summary>
	[Obsolete("This property is included only for legacy reasons.")]
	[JsonPropertyName("packageContent")]
	public string? PackageContent { get; init; }

	[JsonPropertyName("projectUrl")]
	public string? ProjectUrl { get; init; }

	/// <summary>
	/// A string containing a ISO 8601 timestamp of when the package was published.
	/// </summary>
	[JsonPropertyName("published")]
	public DateTimeOffset? Published { get; init; }

	/// <summary>
	/// A URL for the rendered (HTML web page) view of the package README.
	/// </summary>
	[JsonPropertyName("readmeUrl")]
	public string? ReadmeUrl { get; init; }

	[JsonPropertyName("requireLicenseAcceptance")]
	public bool? RequireLicenseAcceptance { get; init; }

	[JsonPropertyName("summary")]
	public string? Summary { get; init; }

	[JsonPropertyName("tags")]
	public string? Tags { get; init; }

	[JsonPropertyName("title")]
	public string? Title { get; init; }

	/// <summary>
	/// The full version after normalization.
	/// </summary>
	[JsonPropertyName("version")]
	public required string Version { get; init; }

	/// <summary>
	/// The security vulnerabilities of the package.
	/// </summary>
	[JsonPropertyName("vulnerabilities")]
	public IEnumerable<NugetPackageVulnerabilityDto>? Vulnerabilities { get; init; }
}
