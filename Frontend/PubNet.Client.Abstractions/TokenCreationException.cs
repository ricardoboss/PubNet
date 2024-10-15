namespace PubNet.Client.Abstractions;

public class TokenCreationException : ApiClientException
{
	public TokenCreationException(string message) : base(message)
	{
	}

	public TokenCreationException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
