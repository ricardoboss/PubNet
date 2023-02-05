using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class AuthorDto
{
	public static AuthorDto? FromAuthor(Author? author, bool ignorePackages = false)
	{
		if (author is null) return null;

		return new() {
			UserName = author.UserName,
			Name = author.Name,
			Website = author.Website,
			Inactive = author.Inactive,
			RegisteredAt = author.RegisteredAtUtc,
			Packages = !ignorePackages && author.Packages.Any() ? author.Packages.Select(PackageDto.FromPackage) : null
		};
	}

	public string UserName { get; init; }

	public string Name { get; init; }

	public string? Website { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Inactive { get; init; }

	public DateTimeOffset RegisteredAt { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public IEnumerable<PackageDto>? Packages { get; init; }
}
