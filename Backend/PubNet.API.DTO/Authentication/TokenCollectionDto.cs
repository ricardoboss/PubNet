using System.ComponentModel.DataAnnotations;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.DTO.Authentication;

public class TokenCollectionDto
{
	public static TokenCollectionDto MapFrom(IEnumerable<Token> tokens, Guid? currentTokenId)
	{
		var tokenDtos = tokens.Select(TokenDto.MapFrom);

		return new()
		{
			Tokens = tokenDtos,
			CurrentTokenId = currentTokenId,
		};
	}

	[Required]
	public required IEnumerable<TokenDto> Tokens { get; init; } = [];

	public Guid? CurrentTokenId { get; init; }
}
