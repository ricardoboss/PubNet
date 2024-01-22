using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Authentication;

public interface IAccessTokenService
{
	/// <exception cref="UserNameNotFoundException">Thrown when the user name is not found.</exception>
	/// <exception cref="NoAuthorIdentityException">Thrown when the author has no identity to authenticate with.</exception>
	Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, string ipAddress, string userAgent, CancellationToken cancellationToken = default);

	Task<TokenCreatedDto> CreatePersonalAccessTokenAsync(Identity owner, CreatePersonalAccessTokenDto dto, string ipAddress, string userAgent, CancellationToken cancellationToken = default);
}
