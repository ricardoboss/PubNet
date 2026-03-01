namespace PubNet.SDK.Exceptions;

public class RegistrationsDisabledException(Exception innerException)
	: RegisterException("Registration are currently disabled", innerException);
