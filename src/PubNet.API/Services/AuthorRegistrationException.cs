using PubNet.API.Controllers;

namespace PubNet.API.Services;

/// <summary>
/// A registration was rejected because of what the caller sent. Carries the status and error code to report,
/// so every endpoint creating an account answers the same way for the same problem.
/// </summary>
public sealed class AuthorRegistrationException(int statusCode, string code, string message) : Exception(message)
{
	public int StatusCode { get; } = statusCode;

	public string Code { get; } = code;

	public static AuthorRegistrationException MissingValues =>
		new(PubNetStatusCodes.Status400BadRequest, "missing-values", "Not all required values were supplied");

	public static AuthorRegistrationException UsernameAlreadyInUse =>
		new(PubNetStatusCodes.Status463UsernameAlreadyInUse, "username-already-in-use",
			"The username is already in use");

	public static AuthorRegistrationException EmailAlreadyInUse =>
		new(PubNetStatusCodes.Status464EmailAlreadyInUse, "email-already-in-use",
			"The e-mail address is already in use");
}
