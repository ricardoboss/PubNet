using System.ComponentModel.DataAnnotations;
using PubNet.Auth.Models;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authentication;

[Mapper]
public partial class TokenCreatedDto
{
	[MapProperty(nameof(JsonWebToken.Value), nameof(Token))]
	public static partial TokenCreatedDto MapFrom(JsonWebToken token);

	[Required]
	public required string Token { get; set; }
}
