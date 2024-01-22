using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Queries;

public interface ITokenDao
{
	/// <exception cref="TokenNotFoundException">Thrown if the token is not found.</exception>
	ValueTask<Token> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
