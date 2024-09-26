using PubNet.Database.Entities.Nuget;

namespace PubNet.API.DTO.Packages.Nuget.Spec;

/// <summary>
/// Each item in the index object's <c>items</c> array is a JSON object representing a registration page.
/// </summary>
/// <seealso cref="NugetPackageRegistrationPageDto"/>
public class NugetPackageRegistrationIndexDto
{
	public static NugetPackageRegistrationIndexDto FromNugetPackage(NugetPackage package)
	{
		var items = package.Versions.Select(v => new NugetPackageRegistrationLeafDto
		{
			Id = "",
			CatalogEntry = new NugetCatalogEntryDto
			{
				Id = "null",
				PackageId = v.Package.Name,
				Version = v.Version,
			},
			PackageContent = "",
		}).ToList();

		var page = new NugetPackageRegistrationPageDto
		{
			Id = "",
			Count = items.Count,
			Items = items,
			Lower = items.MinBy(v => v.CatalogEntry.Version)?.CatalogEntry.Version ?? "0.0.0",
			Upper = items.MaxBy(v => v.CatalogEntry.Version)?.CatalogEntry.Version ?? "999.999.999",
		};

		var dto = new NugetPackageRegistrationIndexDto
		{
			Items = [page],
			Count = 1,
		};

		return dto;
	}

	/// <summary>
	/// The number of registration pages in the index.
	/// </summary>
	public required int Count { get; init; }

	/// <summary>
	/// The array of registration pages.
	/// </summary>
	public required IEnumerable<NugetPackageRegistrationPageDto> Items { get; init; } = [];
}
