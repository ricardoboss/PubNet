using JetBrains.Annotations;

namespace PubNet.API.DTO.Authors;

[PublicAPI]
public record DeleteAuthorRequestDto(string Password);
