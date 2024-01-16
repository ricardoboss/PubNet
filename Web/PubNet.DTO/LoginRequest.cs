using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO;

public class LoginRequest
{
	[Required]
	[EmailAddress(ErrorMessage = "Invalid email.")]
	public string? Email { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string? Password { get; set; }
}
