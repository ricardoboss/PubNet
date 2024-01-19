using System.Text.Json.Serialization;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// <para>
/// The <c>targetFramework</c> string uses the format implemented by NuGet's .NET library
/// <a href="https://www.nuget.org/packages/NuGet.Frameworks/">NuGet.Frameworks</a>. If no <c>targetFramework</c> is
/// specified, the dependency group applies to all target frameworks.
/// </para>
/// <para>
/// The <c>dependencies</c> property is an array of objects, each representing a package dependency of the current
/// package.
/// </para>
/// </summary>
public class NugetPackageDependencyGroupDto
{
	/// <summary>
	/// The target framework that these dependencies are applicable to.
	/// </summary>
	[JsonPropertyName("targetFramework")]
	public string? TargetFramework { get; init; }

	[JsonPropertyName("dependencies")]
	public IEnumerable<NugetPackageDependencyDto>? Dependencies { get; init; }
}
