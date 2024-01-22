namespace PubNet.API.DTO.Authentication;

public class CreateAccountDto
{
	public required string UserName { get; init; }

	public required string Email { get; init; }

	public required string Password { get; init; }
}
