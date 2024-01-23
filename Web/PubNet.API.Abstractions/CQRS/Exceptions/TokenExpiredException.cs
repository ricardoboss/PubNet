using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Exceptions;

public class TokenExpiredException(Token token) : Exception($"Token {token.Id} has expired.");
