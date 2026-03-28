using JetBrains.Annotations;

namespace PubNet.API.DTO.Errors;

[PublicAPI]
public class ValidationErrorsDto
{
	public required IDictionary<string, string[]> Errors { get; init; }

	public required string Title { get; init; }
}
