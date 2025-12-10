using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication;

[PublicAPI]
public class LoginRequest
{
	[EmailAddress(ErrorMessage = "Invalid email.")]
	public string? Email { get; set; }

	[DataType(DataType.Password)]
	public string? Password { get; set; }
}
