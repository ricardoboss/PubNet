namespace PubNet.API.Services;

/// <summary>
/// A password reset was attempted with a token that is unknown, expired or has already been used. The three
/// cases are deliberately indistinguishable to the caller, so a token cannot be probed for why it stopped
/// working.
/// </summary>
public sealed class InvalidPasswordResetTokenException()
	: Exception("The password reset token is invalid, expired or has already been used");
