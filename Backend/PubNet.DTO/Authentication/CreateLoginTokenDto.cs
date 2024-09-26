using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Authentication;

public class CreateLoginTokenDto
{
	[Required, EmailAddress]
	public required string Email { get; init; }

	[Required, DataType(DataType.Password), PasswordPropertyText(true)]
	public required string Password { get; init; }
}
