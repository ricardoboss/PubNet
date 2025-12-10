using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication;

[PublicAPI]
public record JwtTokenResponse(string Token, DateTimeOffset ExpiresAt);
