namespace PubNet.API.Abstractions.Commands.Authentication;

public class CreateTokenCommand : ICommand
{
	public Guid Id { get; } = Guid.NewGuid();

	public required Guid IdentityId { get; init; }

	public required string Name { get; init; }

	public required string[] Scopes { get; init; }

	public required TimeSpan Lifetime { get; init; }
}
