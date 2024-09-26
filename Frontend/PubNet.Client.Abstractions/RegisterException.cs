namespace PubNet.Client.Abstractions;

public class RegisterException : ApiClientException
{
	public RegisterException(string message) : base(message)
	{
	}

	public RegisterException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
