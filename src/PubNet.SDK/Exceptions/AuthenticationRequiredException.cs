namespace PubNet.SDK.Exceptions;

public class AuthenticationRequiredException : PubNetSdkException
{
	public AuthenticationRequiredException(string message) : base(message)
	{
	}

	public AuthenticationRequiredException(Exception innerException) : base("Authentication is required to perform this action", innerException)
	{
	}

	public AuthenticationRequiredException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
