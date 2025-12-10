using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

public interface IAuthenticationService
{
	Task<bool> GetRegistrationsEnabledAsync(CancellationToken cancellationToken = default);

	ValueTask<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default);

	Task<JwtTokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

	Task LogoutAsync(CancellationToken cancellationToken = default);

	Task<AuthorDto?> GetSelfAsync(CancellationToken cancellationToken = default);

	Task<bool> IsSelfAsync(string username, CancellationToken cancellationToken = default);
}
