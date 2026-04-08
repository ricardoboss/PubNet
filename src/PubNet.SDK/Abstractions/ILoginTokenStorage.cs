namespace PubNet.SDK.Abstractions;

public interface ILoginTokenStorage
{
	Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);

	Task StoreTokenAsync(string token, CancellationToken cancellationToken = default);

	Task DeleteTokenAsync(CancellationToken cancellationToken = default);

	event EventHandler<TokenChangedEventArgs>? TokenChanged;
}
