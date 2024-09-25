using System.Security.Authentication;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;
using PubNet.Web;
using PubNet.Web.Models;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.API.Services.Authentication;

public class AccessTokenService(IPasswordVerifier passwordVerifier, ITokenDmo tokenDmo, IIdentityDao identityDao, IJwtFactory jwtFactory) : IAccessTokenService
{
	private static InvalidCredentialException InvalidCredentials => new("The given credentials are incorrect.");

	public async Task<JsonWebToken> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		var identity = await identityDao.TryFindByEmailAsync(dto.Email, cancellationToken);
		if (identity is null)
			throw InvalidCredentials;

		if (!await passwordVerifier.VerifyAsync(identity.Id, dto.Password, cancellationToken))
			throw InvalidCredentials;

		const string loginTokenName = "Login";
		var lifetime = TimeSpan.FromDays(90); // TODO: Make configurable

		// TODO: determine what scopes are needed for website usage
		string[] scopes = [
			Scopes.PersonalAccessTokens.Create, // implies reading
		];

		var token = await tokenDmo.CreateTokenAsync(
			identity,
			loginTokenName,
			scopes.Select(Scope.From),
			lifetime,
			cancellationToken
		);

		return jwtFactory.Create(token);
	}

	public async Task<JsonWebToken> CreatePersonalAccessTokenAsync(Identity owner, CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		var lifetime = TimeSpan.FromDays(dto.LifetimeInDays);

		var token = await tokenDmo.CreateTokenAsync(owner, dto.Name, dto.Scopes.Select(Scope.From), lifetime, cancellationToken);

		return jwtFactory.Create(token);
	}
}
