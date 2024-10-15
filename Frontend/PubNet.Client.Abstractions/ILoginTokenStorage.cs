using PubNet.Auth.Models;

namespace PubNet.Client.Abstractions;

public interface ILoginTokenStorage
{
	Task<JsonWebToken?> GetTokenAsync(CancellationToken cancellationToken = default);

	Task StoreTokenAsync(JsonWebToken token, CancellationToken cancellationToken = default);

	Task DeleteTokenAsync(CancellationToken cancellationToken = default);

	event EventHandler<TokenChangedEventArgs>? TokenChanged;
}
