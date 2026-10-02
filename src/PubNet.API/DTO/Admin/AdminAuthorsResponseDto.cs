using JetBrains.Annotations;
using PubNet.API.DTO.Authors;

namespace PubNet.API.DTO.Admin;

/// <summary>
/// The authors of this instance, including inactive ones and their roles.
/// </summary>
[PublicAPI]
public record AdminAuthorsResponseDto(IEnumerable<AuthorDto> Authors);
