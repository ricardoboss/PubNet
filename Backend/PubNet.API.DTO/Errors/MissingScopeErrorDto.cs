using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Errors;

public class MissingScopeErrorDto : ErrorDto
{
	[Required]
	public required string[] RequiredScopes { get; init; }

	[Required]
	public required string[] GivenScopes { get; init; } = [];

	public string? Message { get; init; }
}
