using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Errors;

public class CodeMessageDto
{
	[Required]
	public required string Code { get; init; }

	[Required]
	public required string Message { get; init; }
}
