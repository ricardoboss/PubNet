namespace PubNet.SDK.Exceptions;

public class UnauthorizedException(string message, Exception innerException)
	: PubNetSdkException(message, innerException);
