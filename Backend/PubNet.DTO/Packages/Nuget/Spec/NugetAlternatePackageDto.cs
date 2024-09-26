namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetAlternatePackageDto
{
	/// <summary>
	/// The ID of the alternate package.
	/// </summary>
	public required string Id { get; init; }

	/// <summary>
	/// The allowed <a href="https://learn.microsoft.com/en-us/nuget/concepts/package-versioning#version-ranges">version
	/// range</a>, or <c>*</c> if any version is allowed.
	/// </summary>
	public string? Range { get; init; }
}
