namespace PubNet.Client.Abstractions;

public abstract class ApiClientException : Exception
{
	protected ApiClientException(string message) : base(message)
	{
	}

	protected ApiClientException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
