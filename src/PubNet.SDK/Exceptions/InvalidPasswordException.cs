namespace PubNet.SDK.Exceptions;

/// <summary>
/// An action that requires confirming the current password was given the wrong one.
/// </summary>
public class InvalidPasswordException(Exception innerException)
	: PubNetSdkException("The password is incorrect", innerException);
