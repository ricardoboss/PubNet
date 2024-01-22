namespace PubNet.API.Abstractions.Queries;

public class TokenNotFoundException(Guid tokenId) : Exception($"Token {tokenId} not found");
