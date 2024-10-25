using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Errors;

public class InternalServerErrorDto
{
	[Required]
	public required string Error { get; init; }

	[Required]
	public required string Message { get; init; }

	public string[]? StackTrace { get; init; }
}
