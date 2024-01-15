using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class AuthorDto
{
	public static AuthorDto? FromAuthor(Author? author, bool ignorePackages = false)
	{
		if (author is null) return null;

		return new()
		{
			UserName = author.UserName,
			Name = author.Name,
			Website = author.Website,
			Inactive = author.Inactive,
			RegisteredAt = author.RegisteredAtUtc,
			Packages = !ignorePackages && author.DartPackages.Any() ? author.DartPackages.Select(PackageDto.FromPackage) : null
		};
	}

	public string UserName { get; init; } = null!;

	public string Name { get; init; } = null!;

	public string? Website { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Inactive { get; init; }

	public DateTimeOffset RegisteredAt { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public IEnumerable<PackageDto>? Packages { get; init; }
}
