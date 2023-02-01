namespace PubNet.API.DTO;

public class BearerTokenException : UnauthorizedAccessException
{
	public BearerTokenException(string message) : base(message)
	{
	}
}
