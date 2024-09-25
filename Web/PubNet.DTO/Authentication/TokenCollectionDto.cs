using System.ComponentModel.DataAnnotations;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.DTO.Authentication;

public class TokenCollectionDto
{
	public static TokenCollectionDto MapFrom(IEnumerable<Token> tokens)
	{
		var tokenDtos = tokens.Select(TokenDto.MapFrom);

		return new()
		{
			Tokens = tokenDtos,
		};
	}

	[Required]
	public required IEnumerable<TokenDto> Tokens { get; init; } = [];
}
