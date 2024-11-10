using NuGet.Packaging.Core;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// If the <c>range</c> property is excluded or an empty string, the client should default to the version range
/// <c>(, )</c>. That is, any version of the dependency is allowed. The value of <c>*</c> is not allowed for the
/// <c>range</c> property.
/// </summary>
[Mapper]
public partial class NugetPackageDependencyDto
{
	public static partial NugetPackageDependencyDto MapFrom(PackageDependency dependency);

	/// <summary>
	/// The ID of the package dependency.
	/// </summary>
	public required string Id { get; init; }

	/// <summary>
	/// The allowed <a href="https://learn.microsoft.com/en-us/nuget/concepts/package-versioning#version-ranges">version
	/// tange</a> of the dependency.
	/// </summary>
	public string? Range { get; init; }

	/// <summary>
	/// The URL to the registration index for this dependency.
	/// </summary>
	public string? Registration { get; init; }
}
