using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Authentication;

public class TokenCreatedDto
{
	[Required]
	public required string Value { get; set; }

	[Required]
	public required TokenDto Token { get; set; }
}
