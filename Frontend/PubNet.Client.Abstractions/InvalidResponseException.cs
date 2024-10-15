namespace PubNet.Client.Abstractions;

public class InvalidResponseException : ApiClientException
{
	public InvalidResponseException(string message) : base(message)
	{
	}

	public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
