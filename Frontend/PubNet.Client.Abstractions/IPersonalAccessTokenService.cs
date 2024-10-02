using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface IPersonalAccessTokenService
{
	/// <exception cref="InvalidResponseException">Thrown when an unexpected response is received.</exception>
	Task<IEnumerable<TokenDto>> GetAsync(CancellationToken cancellationToken = default);

	/// <exception cref="InvalidResponseException">Thrown when an unexpected response is received.</exception>
	/// <exception cref="TokenCreationException">Thrown when the token could not be created.</exception>
	Task<TokenDto> CreateAsync(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default);

	/// <exception cref="InvalidResponseException">Thrown when an unexpected response is received.</exception>
	Task<IReadOnlyCollection<string>> GetAllowedScopesAsync(CancellationToken cancellationToken = default);
}
