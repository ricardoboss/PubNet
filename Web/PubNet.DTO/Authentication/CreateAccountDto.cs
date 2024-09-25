using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Authentication;

public class CreateAccountDto
{
	[Required]
	public required string UserName { get; init; }

	[Required, EmailAddress]
	public required string Email { get; init; }

	[Required, DataType(DataType.Password), PasswordPropertyText(true)]
	public required string Password { get; init; }
}
