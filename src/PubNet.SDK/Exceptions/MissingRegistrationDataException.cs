namespace PubNet.SDK.Exceptions;

public class MissingRegistrationDataException(Exception innerException)
	: RegisterException("Some data for your registration is missing", innerException);
