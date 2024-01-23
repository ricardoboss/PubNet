using System.Security.Authentication;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Authentication;

public interface IAccessTokenService
{
	/// <exception cref="InvalidCredentialException">Thrown when the given credentials are incorrect.</exception>
	Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default);

	Task<TokenCreatedDto> CreatePersonalAccessTokenAsync(Identity owner, CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default);
}
