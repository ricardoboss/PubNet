using Microsoft.Kiota.Abstractions;

namespace PubNet.Client.Abstractions;

public class InvalidResponseException : ApiClientException
{
	public InvalidResponseException(string message) : base(message)
	{
	}

	public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
	{
	}

	public static InvalidResponseException UnexpectedResponse(ApiException e)
	{
		return new($"API returned an unexpected status code: {e.ResponseStatusCode}", e);
	}
}
