using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// <para>
/// The <c>lower</c> and <c>upper</c> bounds of the page object are useful when the metadata for a specific page version
/// is needed. These bounds can be used to fetch the only registration page needed. The version strings adhere to
/// <a href="https://learn.microsoft.com/en-us/nuget/concepts/package-versioning">NuGet's version rules</a>. The version
/// strings are normalized and do not include build metadata. As with all versions in the NuGet ecosystem, comparison of
/// version strings is implemented using <a href="https://semver.org/spec/v2.0.0.html#spec-item-11">SemVer 2.0.0's
/// version precedence rules</a>.
/// </para>
/// <para>
/// The <c>parent</c> property will only appear if the registration page object has the <c>items</c> property.
/// </para>
/// <para>
/// If the <c>items</c> property is not present in the registration page object, the URL specified in the <c>@id</c>
/// must be used to fetch metadata about individual package versions. The <c>items</c> array is sometimes excluded from
/// the page object as an optimization. If the number of versions of a single package ID is very large, then the
/// registration index document will be massive and wasteful to process for a client that only cares about a specific
/// version or small range of versions.
/// </para>
/// <para>
/// Note that if the <c>items</c> property is present, the <c>@id</c> property need not be used, since all of the page
/// data is already inlined in the items property.
/// </para>
/// <para>
/// Each item in the page object's <c>items</c> array is a JSON object representing a registration leaf and it's
/// associated metadata.
/// </para>
/// </summary>
/// <seealso cref="NugetPackageRegistrationIndexDto"/>
public class NugetPackageRegistrationPageDto
{
	/// <summary>
	/// The URL to the registration index.
	/// </summary>
	public string? Parent { get; init; }

	/// <summary>
	/// The URL to the registration page.
	/// </summary>
	[JsonPropertyName("@id")]
	public required string Id { get; init; }

	/// <summary>
	/// The number of registration leaves in the page.
	/// </summary>
	public required int Count { get; init; }

	/// <summary>
	/// The array of registration leaves and their associate metadata.
	/// </summary>
	public IEnumerable<NugetPackageRegistrationLeafDto>? Items { get; init; }

	/// <summary>
	/// The lowest SemVer 2.0.0 version in the page (inclusive).
	/// </summary>
	public required string Lower { get; init; }

	/// <summary>
	/// The highest SemVer 2.0.0 version in the page (inclusive).
	/// </summary>
	public required string Upper { get; init; }
}
