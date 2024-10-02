using System.Security.Authentication;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;
using PubNet.Auth;
using PubNet.Auth.Models;

namespace PubNet.API.Services.Authentication;

public class AccessTokenService(IPasswordVerifier passwordVerifier, ITokenDmo tokenDmo, IIdentityDao identityDao) : IAccessTokenService
{
	private static InvalidCredentialException InvalidCredentials => new("The given credentials are incorrect.");

	public async Task<Token> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
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

		return token;
	}

	public async Task<Token> CreatePersonalAccessTokenAsync(Identity owner, CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		var lifetime = TimeSpan.FromDays(dto.LifetimeInDays);
		if (lifetime <= TimeSpan.Zero)
			throw new ArgumentOutOfRangeException(nameof(dto.LifetimeInDays), lifetime, "Lifetime must be greater than zero");

		if (lifetime.TotalDays > 365)
			throw new ArgumentOutOfRangeException(nameof(dto.LifetimeInDays), lifetime, "Lifetime must be less than or equal to 365 days");

		var scopes = dto.Scopes.Select(Scope.From).ToList();
		if (scopes.Count == 0)
			throw new ArgumentOutOfRangeException(nameof(dto.Scopes), dto.Scopes, "At least one scope must be selected");

		var token = await tokenDmo.CreateTokenAsync(owner, dto.Name, scopes, lifetime, cancellationToken);

		return token;
	}
}
