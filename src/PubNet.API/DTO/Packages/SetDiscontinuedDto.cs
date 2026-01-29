using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages;

[PublicAPI]
public record SetDiscontinuedDto(string? Replacement);
