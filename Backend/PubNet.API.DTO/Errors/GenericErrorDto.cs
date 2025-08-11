namespace PubNet.API.DTO.Errors;

public class GenericErrorDto : ErrorDto
{
	public string[]? StackTrace { get; init; }
}
