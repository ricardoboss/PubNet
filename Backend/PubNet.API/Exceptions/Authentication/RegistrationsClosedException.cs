using PubNet.API.Abstractions;

namespace PubNet.API.Exceptions.Authentication;

public class RegistrationsClosedException()
	: ApiException("registrations-closed", "Registrations are closed.", StatusCodes.Status403Forbidden);
