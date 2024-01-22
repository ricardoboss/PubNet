namespace PubNet.API.DTO.Authentication;

public class TokenCreatedDto
{
	public required string Token { get; set; }

	public required DateTimeOffset ExpiresAtUtc { get; set; }
}
