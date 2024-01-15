using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class PackageDto
{
	public string Name { get; init; } = null!;

	public List<PackageVersionDto>? Versions { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsDiscontinued { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string? ReplacedBy { get; init; }

	public PackageVersionDto? Latest { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public AuthorDto? Author { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Mirrored { get; set; }

	public static PackageDto FromPackage(DartPackage dartPackage)
	{
		var versions = dartPackage.Versions.Any()
			? dartPackage.Versions.Select(PackageVersionDto.FromPackageVersion).ToList()
			: null;

		var latestDto = dartPackage.Latest is null ? null : PackageVersionDto.FromPackageVersion(dartPackage.Latest);

		var authorDto = AuthorDto.FromAuthor(dartPackage.Author, true);

		return new()
		{
			Name = dartPackage.Name,
			Versions = versions,
			IsDiscontinued = dartPackage.IsDiscontinued,
			ReplacedBy = dartPackage.ReplacedBy,
			Latest = latestDto,
			Author = authorDto,
			Mirrored = false,
		};
	}
}
