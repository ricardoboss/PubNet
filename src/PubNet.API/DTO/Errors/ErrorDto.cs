using JetBrains.Annotations;

namespace PubNet.API.DTO.Errors;

[PublicAPI]
public abstract class ErrorDto
{
	public required CodeMessageDto Error { get; init; }
}
