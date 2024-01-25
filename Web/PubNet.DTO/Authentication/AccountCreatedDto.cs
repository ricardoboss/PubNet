using PubNet.Database.Entities.Auth;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authentication;

[Mapper]
public partial class AccountCreatedDto
{
	[MapProperty([nameof(Identity.Email)], [nameof(Email)])]
	[MapProperty([nameof(Identity.Author), nameof(Identity.Author.UserName)], [nameof(UserName)])]
	public static partial AccountCreatedDto MapFrom(Identity identity);

	public required string Email { get; set; }

	public required string UserName { get; set; }
}
