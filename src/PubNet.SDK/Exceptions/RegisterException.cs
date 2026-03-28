namespace PubNet.SDK.Exceptions;

public class RegisterException(string message, Exception innerException) : PubNetSdkException(message, innerException);
