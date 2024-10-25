using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Errors;

public class AuthErrorDto
{
	[Required]
	public required string Error { get; init; }

	[Required]
	public required string Message { get; init; }
}
