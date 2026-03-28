namespace PubNet.SDK.Exceptions;

public class EmailAlreadyRegisteredException(string email, Exception innerException)
	: RegisterException($"The e-mail address '{email}' is already registered", innerException);
