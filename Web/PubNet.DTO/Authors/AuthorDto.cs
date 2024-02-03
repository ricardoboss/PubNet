using PubNet.Database.Entities;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authors;

[Mapper]
public partial class AuthorDto
{
	public static partial AuthorDto MapFrom(Author author);

	public Guid Id { get; set; }

	public string UserName { get; set; } = null!;

	public DateTimeOffset RegisteredAtUtc { get; set; }
}
