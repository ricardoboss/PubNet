namespace PubNet.API.Abstractions.CQRS.Exceptions;

public class IdentityNotFoundException(Guid identityId) : Exception($"Identity {identityId} not found");
