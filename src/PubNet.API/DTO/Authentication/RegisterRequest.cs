using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication;

[PublicAPI]
public class RegisterRequest
{
	[EmailAddress(ErrorMessage = "Invalid email.")]
	public string? Email { get; set; }

	[DataType(DataType.Password)]
	public string? Password { get; set; }

	public string? Username { get; set; }

	public string? Name { get; set; }

	[DataType(DataType.Url)]
	public string? Website { get; set; }
}
