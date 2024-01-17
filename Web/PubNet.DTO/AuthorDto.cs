using System.Text.Json.Serialization;
using PubNet.Database.Entities;

namespace PubNet.API.DTO;

public class AuthorDto
{
	public static AuthorDto? FromAuthor(Author? author, bool ignorePackages = false)
	{
		if (author is null) return null;

		return new()
		{
			UserName = author.UserName,
			RegisteredAt = author.RegisteredAt,
			Packages = !ignorePackages && author.DartPackages.Count != 0 ? author.DartPackages.Select(PackageDto.FromPackage) : null,
		};
	}

	public string UserName { get; init; } = null!;

	public DateTimeOffset RegisteredAt { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public IEnumerable<PackageDto>? Packages { get; init; }
}
