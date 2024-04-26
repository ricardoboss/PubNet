namespace PubNet.API.DTO.Authors;

public class AuthorListDto
{
	public required int TotalHits { get; init; }

	public required IEnumerable<AuthorDto> Authors { get; init; }
}
