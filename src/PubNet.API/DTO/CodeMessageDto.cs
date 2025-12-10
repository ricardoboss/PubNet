using JetBrains.Annotations;

namespace PubNet.API.DTO;

[PublicAPI]
public class CodeMessageDto
{
	public required string Code { get; init; }

	public required string Message { get; init; }
}
