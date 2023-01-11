namespace PubNet.API.DTO;

public record BearerTokenResponse(string Token, DateTimeOffset ExpiresAtUtc);
