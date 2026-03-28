using Microsoft.Kiota.Abstractions;

namespace PubNet.SDK.Exceptions;

public class UnexpectedResponseException : PubNetSdkException
{
	public UnexpectedResponseException(ApiException innerException) : base("Unexpected API response", innerException)
	{
	}

	public UnexpectedResponseException(string message) : base(message)
	{
	}
}
