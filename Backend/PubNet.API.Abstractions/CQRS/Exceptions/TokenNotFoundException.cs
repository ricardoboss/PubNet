namespace PubNet.API.Abstractions.CQRS.Exceptions;

public class TokenNotFoundException(Guid tokenId) : Exception($"Token {tokenId} not found");
