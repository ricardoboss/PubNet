namespace PubNet.SDK.Exceptions;

public class InvalidNewPasswordException(Exception innerException)
	: PubNetSdkException("The new password is invalid", innerException);
