using System.ComponentModel.DataAnnotations;
using PubNet.API.DTO.Authors;
using PubNet.Auth.Models;
using PubNet.Database.Entities.Auth;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Admin;

[Mapper]
[UseStaticMapper<AuthorDto>]
public partial class IdentityDto
{
	[MapperIgnoreSource(nameof(Identity.AuthorId))]
	[MapperIgnoreSource(nameof(Identity.PasswordHash))]
	[MapperIgnoreSource(nameof(Identity.Tokens))]
	public static partial IdentityDto MapFrom(Identity identity);

	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required AuthorDto Author { get; init; }

	[Required, EmailAddress]
	public required string Email { get; init; }

	[Required]
	public required Role Role { get; init; }
}
