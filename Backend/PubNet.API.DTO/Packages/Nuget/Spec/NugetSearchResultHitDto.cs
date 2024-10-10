using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// <para>
/// On nuget.org, a verified package is one which has a package ID matching a reserved ID prefix and owned by one of the
/// reserved prefix's owners. For more information, see the
/// <a href="https://learn.microsoft.com/en-us/nuget/nuget-org/id-prefix-reservation">documentation about ID prefix
/// reservation</a>.
/// </para>
/// </summary>
public class NugetSearchResultHitDto
{
	public static NugetSearchResultHitDto MapFrom(NugetPackageDto package)
	{
		return new()
		{
			Id = package.Name,
			Version = package.Latest?.Version ?? "",
			Versions = package.Versions?.Select(v => NugetSearchResultHitVersionInfoDto.MapFrom(package, v)) ?? [],
			PackageTypes = [],
		};
	}

	/// <summary>
	/// The ID of the matched package.
	/// </summary>
	[JsonPropertyName("@id")]
	public required string Id { get; init; }

	/// <summary>
	/// The full SemVer 2.0.0 version string of the package (could contain build metadata).
	/// </summary>
	public required string Version { get; init; }

	public string? Description { get; init; }

	/// <summary>
	/// All of the versions of the package matching the <c>prerelease</c> parameter
	/// </summary>
	public required IEnumerable<NugetSearchResultHitVersionInfoDto> Versions { get; init; } = [];

	public string[]? Authors { get; init; }

	public string? IconUrl { get; init; }

	public string? LicenseUrl { get; init; }

	public string[]? Owners { get; init; }

	public string? ProjectUrl { get; init; }

	/// <summary>
	/// The absolute URL to the associated
	/// <a href="https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource#registration-index">
	/// registration index</a>
	/// </summary>
	public string? Registration { get; init; }

	public string? Summary { get; init; }

	public string[]? Tags { get; init; }

	public string? Title { get; init; }

	/// <summary>
	/// This value can be inferred by the sum of downloads in the <c>versions</c> array.
	/// </summary>
	public long? TotalDownloads { get; init; }

	/// <summary>
	/// A JSON boolean indicating whether the package is
	/// <a href="https://learn.microsoft.com/en-us/nuget/nuget-org/id-prefix-reservation">verified</a>.
	/// </summary>
	public bool? Verified { get; init; }

	/// <summary>
	/// The package types defined by the package author (added in <c>SearchQueryService/3.5.0</c>)
	/// </summary>
	public required IEnumerable<NugetSearchResultHitPackageTypeDto> PackageTypes { get; init; } = [];
}
