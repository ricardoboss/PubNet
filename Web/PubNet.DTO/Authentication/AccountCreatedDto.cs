using System.ComponentModel.DataAnnotations;
using PubNet.Database.Entities.Auth;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authentication;

[Mapper]
public partial class AccountCreatedDto
{
	[MapProperty([nameof(Identity.Email)], [nameof(Email)])]
	[MapProperty([nameof(Identity.Author), nameof(Identity.Author.UserName)], [nameof(UserName)])]
	public static partial AccountCreatedDto MapFrom(Identity identity);

	[Required, EmailAddress]
	public required string Email { get; set; }

	[Required]
	public required string UserName { get; set; }
}
