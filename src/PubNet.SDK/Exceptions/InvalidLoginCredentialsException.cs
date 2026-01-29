namespace PubNet.SDK.Exceptions;

public class InvalidLoginCredentialsException(string message, Exception innerException)
	: PubNetSdkException(message, innerException);
