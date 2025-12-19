namespace PubNet.SDK.Exceptions;

public class PubNetSdkException : Exception
{
	public PubNetSdkException(string message) : base(message)
	{
	}

	public PubNetSdkException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
