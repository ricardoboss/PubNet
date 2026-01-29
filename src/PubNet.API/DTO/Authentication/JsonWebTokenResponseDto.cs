using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication;

[PublicAPI]
public record JsonWebTokenResponseDto(string Token, DateTimeOffset ExpiresAt);
