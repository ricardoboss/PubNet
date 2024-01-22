namespace PubNet.API.DTO.Authentication;

public class CreateLoginTokenDto
{
	public required string Username { get; init; }

	public required string Password { get; init; }
}
