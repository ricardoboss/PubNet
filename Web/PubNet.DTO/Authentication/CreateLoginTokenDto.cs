namespace PubNet.API.DTO.Authentication;

public class CreateLoginTokenDto
{
	public required string Email { get; init; }

	public required string Password { get; init; }
}
