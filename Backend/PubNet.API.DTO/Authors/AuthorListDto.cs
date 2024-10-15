using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Authors;

public class AuthorListDto
{
	[Required]
	public required int TotalHits { get; init; }

	[Required]
	public required IEnumerable<AuthorDto> Authors { get; init; }
}
