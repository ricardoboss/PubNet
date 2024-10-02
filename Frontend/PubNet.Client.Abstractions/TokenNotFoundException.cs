namespace PubNet.Client.Abstractions;

public class TokenNotFoundException : ApiClientException
{
	public TokenNotFoundException(string message) : base(message)
	{
	}

	public TokenNotFoundException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
