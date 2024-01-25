using System.Security.Authentication;
using PubNet.API.DTO.Authentication;
using PubNet.Database.Entities.Auth;
using PubNet.Web.Abstractions.Models;

namespace PubNet.API.Abstractions.Authentication;

public interface IAccessTokenService
{
	/// <exception cref="InvalidCredentialException">Thrown when the given credentials are incorrect.</exception>
	Task<JsonWebToken> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default);

	Task<JsonWebToken> CreatePersonalAccessTokenAsync(Identity owner, CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default);
}
