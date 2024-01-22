using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Commands;

public interface ITokenDmo
{
	Task<Token> CreateTokenAsync(Identity owner, string name, string ipAddress, string userAgent, string[] scopes,
		TimeSpan lifetime, CancellationToken cancellationToken = default);
}
