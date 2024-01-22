using PubNet.API.Abstractions.Authentication;
using PubNet.API.DTO.Authentication;

namespace PubNet.API.Services;

public class AuthenticationService : IAuthenticationService
{
	public Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
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
