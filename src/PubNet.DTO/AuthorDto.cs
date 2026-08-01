using System.Text.Json.Serialization;
using PubNet.Database.Models;

namespace PubNet.API.DTO;

public class AuthorDto
{
	public static AuthorDto? FromAuthor(Author? author, bool ignorePackages = false,
		bool includePrivateData = false)
	{
		if (author is null) return null;

		return new()
		{
			UserName = author.UserName,
			Name = author.Name,
			Website = author.Website,
			Inactive = author.Inactive,
			RegisteredAt = author.RegisteredAtUtc,
			// not published: knowing who the admins are is a head start for an attacker
			Role = includePrivateData ? author.Role : null,
			Packages = !ignorePackages && author.Packages.Count != 0
				? author.Packages.Select(PackageDto.FromPackage)
				: null,
		};
	}

	public string UserName { get; init; } = null!;

	public string Name { get; init; } = null!;

	public string? Website { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool Inactive { get; init; }

	public DateTimeOffset RegisteredAt { get; init; }

	/// <summary>
	/// Only populated when an author reads their own profile.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public Role? Role { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public IEnumerable<PackageDto>? Packages { get; init; }
}
