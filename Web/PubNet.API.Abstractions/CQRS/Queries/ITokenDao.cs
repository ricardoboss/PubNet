using PubNet.API.Abstractions.CQRS.Exceptions;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Queries;

public interface ITokenDao
{
	/// <exception cref="TokenNotFoundException">Thrown if the token is not found.</exception>
	ValueTask<Token> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
