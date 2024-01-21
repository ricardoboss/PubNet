namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// Each item in the index object's <c>items</c> array is a JSON object representing a registration page.
/// </summary>
/// <seealso cref="NugetPackageRegistrationPageDto"/>
public class NugetPackageRegistrationIndexDto
{
	/// <summary>
	/// The number of registration pages in the index.
	/// </summary>
	public required int Count { get; init; }

	/// <summary>
	/// The array of registration pages.
	/// </summary>
	public required IEnumerable<NugetPackageRegistrationPageDto> Items { get; init; } = [];
}
