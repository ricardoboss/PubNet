using JetBrains.Annotations;

namespace PubNet.API.DTO.Errors;

[PublicAPI]
public class StacktraceErrorDto : ErrorMessageDto
{
	public string[]? StackTrace { get; init; }
}
