using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO;

public class GenericErrorDto
{
	[Required]
	public required GenericErrorContentDto Error { get; init; }
}

public class GenericErrorContentDto
{
	[Required]
	public required string Code { get; init; }

	[Required]
	public required string Message { get; init; }

	public string[]? StackTrace { get; init; }
}
