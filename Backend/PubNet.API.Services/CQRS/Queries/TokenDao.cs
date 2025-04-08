using PubNet.API.Abstractions.CQRS.Exceptions;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.CQRS.Queries;

public class TokenDao(PubNet2Context context) : ITokenDao
{
	public async ValueTask<Token> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var token = await context.Tokens.FindAsync([id], cancellationToken);
		if (token is null)
			throw new TokenNotFoundException(id);

		if (token.ExpiresAtUtc < DateTimeOffset.UtcNow)
			throw new TokenExpiredException(token);

		return token;
	}
}
