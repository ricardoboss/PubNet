using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO;

public class RegisterRequest
{
	[Required]
	[EmailAddress(ErrorMessage = "Invalid email.")]
	public string? Email { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string? Password { get; set; }

	[Required]
	public string? Username { get; set; }

	[Required]
	public string? Name { get; set; }

	[DataType(DataType.Url)]
	public string? Website { get; set; }
}
