using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Errors;

public class MissingScopeErrorDto
{
	[Required]
	public required string[] RequiredScopes { get; init; }

	[Required]
	public required string[] GivenScopes { get; init; } = [];
}
