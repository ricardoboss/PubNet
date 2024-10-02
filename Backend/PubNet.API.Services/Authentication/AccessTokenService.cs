using System.Security.Authentication;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.Abstractions.CQRS.Exceptions;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;
using PubNet.Auth;
using PubNet.Auth.Models;

namespace PubNet.API.Services.Authentication;

public class AccessTokenService(IPasswordVerifier passwordVerifier, ITokenDmo tokenDmo, ITokenDao tokenDao, IIdentityDao identityDao) : IAccessTokenService
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
		List<string> scopes = [
			Scopes.PersonalAccessTokens.Create,
			Scopes.PersonalAccessTokens.Read,
			Scopes.PersonalAccessTokens.Delete,
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

		var sanitizedScopes = dto.Scopes.Where(AllowedScopes.Contains).Select(Scope.From).ToList();
		if (sanitizedScopes.Count == 0)
			throw new ArgumentOutOfRangeException(nameof(dto.Scopes), dto.Scopes, "At least one valid scope must be selected");

		var canonicalizedScopes = new List<Scope>();
		foreach (var scope in sanitizedScopes)
		{
			if (canonicalizedScopes.Any(s => s.IsParentOf(scope)))
				continue;

			// remove all scopes that are children of the current scope
			canonicalizedScopes
				.Where(s => scope.IsParentOf(s))
				.ToList()
				.ForEach(s => canonicalizedScopes.Remove(s));

			canonicalizedScopes.Add(scope);
		}

		var token = await tokenDmo.CreateTokenAsync(owner, dto.Name, canonicalizedScopes, lifetime, cancellationToken);

		return token;
	}

	public IEnumerable<string> AllowedScopes => [
		Scopes.PersonalAccessTokens.Create,
		Scopes.PersonalAccessTokens.Read,
		Scopes.PersonalAccessTokens.Delete,
		Scopes.PersonalAccessTokens.Any,
		Scopes.Dart.New,
		Scopes.Dart.Discontinue,
		Scopes.Dart.Retract,
		Scopes.Dart.Any,
		Scopes.Nuget.New,
		Scopes.Nuget.Delete,
		Scopes.Nuget.Any,
		Scopes.Authors.Delete.Self,
	];

	public async Task<Token?> GetTokenAsync(Guid tokenId, CancellationToken cancellationToken = default)
	{
		try
		{
			return await tokenDao.FindByIdAsync(tokenId, cancellationToken);
		}
		catch (Exception e) when (e is TokenNotFoundException or TokenExpiredException)
		{
			return null;
		}
	}

	public async Task DeleteTokenAsync(Token token, CancellationToken cancellationToken = default)
	{
		await tokenDmo.DeleteTokenAsync(token, cancellationToken);
	}
}
