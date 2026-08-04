namespace PubNet.SDK.Exceptions;

/// <summary>
/// No account exists for the given e-mail address.
/// </summary>
public class EmailNotFoundException(string email, Exception innerException)
	: PubNetSdkException($"No account exists for the e-mail address {email}", innerException);
