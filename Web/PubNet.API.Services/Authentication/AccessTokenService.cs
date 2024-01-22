using System.Security.Authentication;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;
using PubNet.Web.Abstractions;

namespace PubNet.API.Services.Authentication;

public class AccessTokenService(IPasswordVerifier passwordVerifier, ITokenDmo tokenDmo, IAuthorDao authorDao, IJwtFactory jwtFactory) : IAccessTokenService
{
	public async Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, string ipAddress, string userAgent, CancellationToken cancellationToken = default)
	{
		var author = await authorDao.TryFindByUsernameAsync(dto.Username, cancellationToken);
		if (author is null)
			throw new UserNameNotFoundException(dto.Username);

		if (author.Identity is not {} identity)
			throw new NoAuthorIdentityException(author);

		if (!await passwordVerifier.VerifyAsync(identity.Id, dto.Password, cancellationToken))
			throw new InvalidCredentialException("The given password is incorrect.");

		const string loginTokenName = "Login";
		var lifetime = TimeSpan.FromDays(90); // TODO: Make configurable
		string[] scopes = ["all"]; // TODO: replace with actual scopes

		var token = await tokenDmo.CreateTokenAsync(identity, loginTokenName, ipAddress, userAgent, scopes, lifetime, cancellationToken);

		var jwt = jwtFactory.Create(token);

		return new()
		{
			Token = jwt,
			ExpiresAtUtc = token.ExpiresAtUtc,
		};
	}

	public async Task<TokenCreatedDto> CreatePersonalAccessTokenAsync(Identity owner, CreatePersonalAccessTokenDto dto, string ipAddress, string userAgent, CancellationToken cancellationToken = default)
	{
		var lifetime = TimeSpan.FromDays(dto.LifetimeInDays);

		var token = await tokenDmo.CreateTokenAsync(owner, dto.Name, ipAddress, userAgent, dto.Scopes, lifetime, cancellationToken);

		var jwt = jwtFactory.Create(token);

		return new()
		{
			Token = jwt,
			ExpiresAtUtc = token.ExpiresAtUtc,
		};
	}
}
