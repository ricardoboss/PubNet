using PubNet.API.Abstractions;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.CQRS.Commands;

public class TokenDmo(PubNetContext context, ISecureTokenGenerator tokenGenerator, IClientInformationProvider clientInformationProvider) : ITokenDmo
{
	public async Task<Token> CreateTokenAsync(
		Identity owner,
		string name,
		string[] scopes,
		TimeSpan lifetime,
		CancellationToken cancellationToken = default
	)
	{
		var token = new Token
		{
			Identity = owner,
			Name = name,
			Value = tokenGenerator.GenerateSecureToken(ITokenDmo.TokenLength),
			Scopes = scopes,
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
}
