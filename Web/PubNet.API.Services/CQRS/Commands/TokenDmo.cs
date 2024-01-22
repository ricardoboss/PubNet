using PubNet.API.Abstractions;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.CQRS.Commands;

public class TokenDmo(PubNetContext context, ISecureTokenGenerator tokenGenerator) : ITokenDmo
{
	private const int TokenLength = 32;

	public async Task<Token> CreateTokenAsync(
		Identity owner,
		string name,
		string ipAddress,
		string userAgent,
		string[] scopes,
		TimeSpan lifetime,
		CancellationToken cancellationToken = default
	)
	{
		var token = new Token
		{
			Identity = owner,
			Name = name,
			Value = tokenGenerator.GenerateSecureToken(TokenLength),
			ExpiresAtUtc = DateTimeOffset.UtcNow + lifetime,
			Scopes = scopes.ToArray(),
			CreatedAtUtc = DateTimeOffset.UtcNow,
		};

		await context.Tokens.AddAsync(token, cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		return token;
	}
}
