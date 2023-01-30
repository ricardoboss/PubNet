using PubNet.Models;

namespace PubNet.API.DTO;

public record AuthorsResponse(IEnumerable<SearchResultAuthor> Authors);
