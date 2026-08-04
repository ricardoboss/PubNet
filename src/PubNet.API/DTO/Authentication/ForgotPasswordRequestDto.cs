using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication;

[PublicAPI]
public class ForgotPasswordRequestDto
{
	[EmailAddress(ErrorMessage = "Invalid email.")]
	public string? Email { get; set; }
}
