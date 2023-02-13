namespace PubNet.API.DTO;

public record BearerTokenResponse(string Name, string Token, DateTimeOffset ExpiresAtUtc);
