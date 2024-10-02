using System.Security.Authentication;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Authentication;

public interface IAccessTokenService
{
	/// <exception cref="InvalidCredentialException">Thrown when the given credentials are incorrect.</exception>
	Task<Token> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default);

	/// <exception cref="ArgumentOutOfRangeException">Thrown when the given lifetime is invalid.</exception>
	Task<Token> CreatePersonalAccessTokenAsync(Identity owner, CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default);
}
