using Microsoft.Kiota.Abstractions.Authentication;

namespace PubNet.Frontend.Interfaces;

public interface IAccessTokenStorage : IAccessTokenProvider
{
	ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default);

	Task RemoveTokenAsync(CancellationToken cancellationToken = default);

	Task StoreTokenAsync(string token, CancellationToken cancellationToken = default);
}
