namespace PubNet.API.Exceptions.Authentication;

public class RegistrationsClosedException() : ApiException("Registrations are closed.", StatusCodes.Status403Forbidden);
