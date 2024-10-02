using PubNet.API.Abstractions.CQRS.Exceptions;
using PubNet.Database.Entities.Auth;
using PubNet.Auth.Models;

namespace PubNet.API.Abstractions.CQRS.Commands;

public interface ITokenDmo
{
	const int TokenLength = 32;

	Task<Token> CreateTokenAsync(Identity owner, string name, IEnumerable<Scope> scopes,
		TimeSpan lifetime, CancellationToken cancellationToken = default);

	/// <exception cref="TokenNotFoundException">Thrown when the token could not be found.</exception>
	Task DeleteTokenAsync(Token token, CancellationToken cancellationToken = default);
}
