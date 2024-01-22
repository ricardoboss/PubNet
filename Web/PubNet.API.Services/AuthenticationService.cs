using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.Commands;
using PubNet.API.Abstractions.Commands.Authentication;
using PubNet.API.Abstractions.Queries;
using PubNet.API.DTO.Authentication;

namespace PubNet.API.Services;

public class AuthenticationService(IAsyncCommandHandler<CreateTokenCommand, TokenCreatedResult> createTokenAsyncCommandHandler, ITokenDao tokenDao) : IAuthenticationService
{
	public async Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		// TODO: validate username/password
		var identityId = Guid.NewGuid(); // TODO: get identity id from database

		var command = new CreateTokenCommand
		{
			Name = "Login",
			Lifetime = TimeSpan.FromDays(90), // TODO: make this configurable
			Scopes = [
				// TODO: add all scopes required for website interaction
			],
			IdentityId = identityId,
		};

		var result = await createTokenAsyncCommandHandler.HandleAsync(command, cancellationToken);

		var token = await tokenDao.FindByIdAsync(result.TokenId, cancellationToken);

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
