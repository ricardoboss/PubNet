using PubNet.API.Abstractions;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.Auth.Models;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.CQRS.Commands;

public class TokenDmo(PubNetContext context, ISecureTokenGenerator tokenGenerator, IClientInformationProvider clientInformationProvider) : ITokenDmo
{
	public async Task<Token> CreateTokenAsync(
		Identity owner,
		string name,
		IEnumerable<Scope> scopes,
		TimeSpan lifetime,
		CancellationToken cancellationToken = default
	)
	{
		var tokenValue = tokenGenerator.GenerateSecureToken(ITokenDmo.TokenLength);
		var scopesList = scopes.Select(s => s.Value).ToList();

		var token = new Token
		{
			Identity = owner,
			Name = name,
			Value = tokenValue,
			Scopes = scopesList,
			IpAddress = clientInformationProvider.IpAddress,
			UserAgent = clientInformationProvider.UserAgent,
			DeviceType = clientInformationProvider.DeviceType,
			Browser = clientInformationProvider.Browser,
			Platform = clientInformationProvider.Platform,
			ExpiresAtUtc = DateTimeOffset.UtcNow + lifetime,
			CreatedAtUtc = DateTimeOffset.UtcNow,
		};

		await context.Tokens.AddAsync(token, cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(token).ReloadAsync(cancellationToken);

		return token;
	}

	public async Task DeleteTokenAsync(Token token, CancellationToken cancellationToken = default)
	{
		context.Tokens.Remove(token);

		await context.SaveChangesAsync(cancellationToken);
	}
}
