using PubNet.API.Abstractions.CQRS.Exceptions;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Queries;

public class TokenDao(PubNetContext context) : ITokenDao
{
	public async ValueTask<Token> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var token = await context.Tokens.FindAsync([id], cancellationToken);
		if (token is null)
			throw new TokenNotFoundException(id);

		return token;
	}
}
