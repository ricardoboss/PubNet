using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services;

public class AuthenticationService(ITokenDmo tokenDmo) : IAuthenticationService
{
	public async Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		// TODO: validate username/password
		Identity identity = null!; // TODO: get identity id from database
		var name = "Login";
		var lifetime = TimeSpan.FromDays(90);
		string[] scopes = ["urn:pubnet:author:username:read"];

		var token = await tokenDmo.CreateTokenAsync(identity, name, lifetime, scopes, cancellationToken);

		return new()
		{
			Token = token.Value,
			ExpiresAtUtc = token.ExpiresAtUtc,
		};
	}

	public Task<TokenCreatedDto> CreatePersonalAccessTokenAsync(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
