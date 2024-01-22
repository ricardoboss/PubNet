using PubNet.API.DTO.Authentication;

namespace PubNet.API.Abstractions.Authentication;

public interface IAuthenticationService
{
	Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default);

	Task<TokenCreatedDto> CreatePersonalAccessTokenAsync(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default);

	Task CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default);
}
