namespace PubNet.SDK.Exceptions;

/// <summary>
/// A password reset was attempted with a link that is invalid, expired or has already been used.
/// </summary>
public class InvalidPasswordResetTokenException(Exception innerException)
	: PubNetSdkException("The password reset link is invalid, expired or has already been used", innerException);
