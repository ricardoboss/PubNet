namespace PubNet.SDK.Exceptions;

public class UsernameAlreadyRegisteredException(string username, Exception innerException)
	: RegisterException($"The user name '{username}' is already registered", innerException);
