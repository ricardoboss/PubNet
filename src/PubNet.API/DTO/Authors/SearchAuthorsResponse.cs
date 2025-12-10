using JetBrains.Annotations;

namespace PubNet.API.DTO.Authors;

[PublicAPI]
public record AuthorsResponse(IEnumerable<SearchResultAuthor> Authors);
