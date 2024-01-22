namespace PubNet.API.DTO.Authentication;

public class CreatePersonalAccessTokenDto
{
	public required string Name { get; init; }

	public string[] Scopes { get; init; } = [];

	public required int LifetimeInDays { get; init; }
}
