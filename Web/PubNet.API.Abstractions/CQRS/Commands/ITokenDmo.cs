using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Commands;

public interface ITokenDmo
{
	Task<Token> CreateTokenAsync(Identity owner, string name, TimeSpan lifetime, IEnumerable<string> scopes, CancellationToken cancellationToken = default);
}
