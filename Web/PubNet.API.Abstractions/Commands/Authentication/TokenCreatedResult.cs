namespace PubNet.API.Abstractions.Commands.Authentication;

public class TokenCreatedResult
{
	public required Guid TokenId { get; init; }
}
