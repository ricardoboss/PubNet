using PubNet.Database.Models;

namespace PubNet.API.DTO;

public record AuthorDto(string UserName, string Name, string? Website, bool Inactive, DateTimeOffset RegisteredAt, IEnumerable<PackageDto>? Packages)
{
	public static AuthorDto? FromAuthor(Author? author, bool ignorePackages = false)
	{
		if (author is null) return null;

		return new(
			author.UserName,
			author.Name,
			author.Website,
			author.Inactive,
			author.RegisteredAtUtc,
			!ignorePackages && author.Packages.Any() ? author.Packages.Select(PackageDto.FromPackage) : null
		);
	}
}
