namespace PubNet.API.DTO;

public record JwtTokenResponse(string Token, DateTimeOffset ExpiresAt);
