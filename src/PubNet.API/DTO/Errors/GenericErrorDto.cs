using JetBrains.Annotations;

namespace PubNet.API.DTO.Errors;

[PublicAPI]
public class GenericErrorDto : ErrorDto
{
	public string[]? StackTrace { get; init; }
}
