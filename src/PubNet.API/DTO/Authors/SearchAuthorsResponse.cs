using JetBrains.Annotations;

namespace PubNet.API.DTO.Authors;

[PublicAPI]
public record AuthorsResponseDto(IEnumerable<SearchResultAuthorDto> Authors);
