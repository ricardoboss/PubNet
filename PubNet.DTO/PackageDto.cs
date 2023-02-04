using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class PackageDto
{
	public string Name { get; init; }

	public IEnumerable<PackageVersionDto>? Versions { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsDiscontinued { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string? ReplacedBy { get; init; }

	public PackageVersionDto? Latest { get; init; }

	public AuthorDto? Author { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Mirrored { get; set; }

	public static PackageDto FromPackage(Package package)
	{
		var versions = package.Versions.Any()
			? package.Versions.Select(PackageVersionDto.FromPackageVersion)
			: null;

		return new()
		{
			Name = package.Name,
			Versions = versions,
			IsDiscontinued = package.IsDiscontinued,
			ReplacedBy = package.ReplacedBy,
			Latest = package.Latest is null ? null : PackageVersionDto.FromPackageVersion(package.Latest),
			Author = AuthorDto.FromAuthor(package.Author, true),
			Mirrored = false,
		};
	}
}
