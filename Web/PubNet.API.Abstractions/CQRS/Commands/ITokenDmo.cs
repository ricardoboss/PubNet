using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Commands;

public interface ITokenDmo
{
	const int TokenLength = 32;

	Task<Token> CreateTokenAsync(Identity owner, string name, string[] scopes,
		TimeSpan lifetime, CancellationToken cancellationToken = default);
}
