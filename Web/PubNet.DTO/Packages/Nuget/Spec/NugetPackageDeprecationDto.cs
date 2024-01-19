using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// <para>
/// The reasons property must contain at least one string and should only contains strings from the following table:
/// </para>
/// <list type="table">
///     <listheader>
///         <term>Reason</term>
///         <description>Description</description>
///     </listheader>
///     <item>
///         <term>Legacy</term>
///         <description>The package is no longer maintained.</description>
///     </item>
///     <item>
///         <term>CriticalBugs</term>
///         <description>The package has bugs which make it unsuitable for usage.</description>
///     </item>
///     <item>
///         <term>Other</term>
///         <description>The package is deprecated due to a reason not on this list.</description>
///     </item>
/// </list>
/// <para>
/// If the <c>reasons</c> property contains strings that are not from the known set, they should be ignored. The strings
/// are case-insensitive, so <c>legacy</c> should be treated the same as <c>Legacy</c>. There is no ordering restriction
/// on the array, so the strings can arranged in any arbitrary order. Additionally, if the property contains only
/// strings that are not from the known set, it should be treated as if it only contained the "Other" string.
/// </para>
/// </summary>
public class NugetPackageDeprecationDto
{
	/// <summary>
	/// The reasons why the package was deprecated.
	/// </summary>
	[JsonPropertyName("reasons")]
	public required IEnumerable<string> Reasons { get; init; } = [];

	/// <summary>
	/// The additional details about this deprecation.
	/// </summary>
	[JsonPropertyName("message")]
	public string? Message { get; init; }

	/// <summary>
	/// The alternate package that should be used instead.
	/// </summary>
	[JsonPropertyName("alternatePackage")]
	public NugetAlternatePackageDto? AlternatePackage { get; init; }
}
