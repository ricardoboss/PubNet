using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Errors;

public abstract class ErrorDto
{
	[Required]
	public required CodeMessageDto Error { get; init; }
}
