using JetBrains.Annotations;

namespace PubNet.API.DTO;

[PublicAPI]
public class SuccessMessageDto
{
	public required CodeMessageDto Success { get; init; }
}
