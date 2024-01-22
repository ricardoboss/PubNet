namespace PubNet.API.DTO;

public class GenericErrorDto
{
	public required GenericErrorContentDto Error { get; init; }
}

public class GenericErrorContentDto
{
	public required string Code { get; init; }

	public required string Message { get; init; }

	public string[]? StackTrace { get; init; }
}
