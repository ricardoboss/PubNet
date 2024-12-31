using System.ComponentModel.DataAnnotations;
using PubNet.Database.Entities.Auth;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authentication;

[Mapper]
public partial class AccountCreatedDto
{
	[MapperIgnoreSource(nameof(Identity.Id))]
	[MapperIgnoreSource(nameof(Identity.AuthorId))]
	[MapProperty([nameof(Identity.Author), nameof(Identity.Author.UserName)], nameof(UserName))]
	[MapProperty(nameof(Identity.Email), nameof(Email))]
	[MapperIgnoreSource(nameof(Identity.PasswordHash))]
	[MapperIgnoreSource(nameof(Identity.Role))]
	[MapperIgnoreSource(nameof(Identity.Tokens))]
	public static partial AccountCreatedDto MapFrom(Identity identity);

	[Required, EmailAddress]
	public required string Email { get; set; }

	[Required]
	public required string UserName { get; set; }
}
