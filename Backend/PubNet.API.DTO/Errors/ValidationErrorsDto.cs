using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Errors;

public class ValidationErrorsDto
{
	[Required]
	public required Dictionary<string, string[]> Errors { get; init; } = [];

	[Required]
	public required string Title { get; init; }
}
