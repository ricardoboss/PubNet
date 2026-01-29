using Blazored.LocalStorage;
using PubNet.SDK.Abstractions;

namespace PubNet.Frontend.Services;

public class BrowserLoginTokenStorage(ILocalStorageService localStorageService) : ILoginTokenStorage
{
	private const string StorageKey = "loginToken";

	public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
	{
		var tokenString = await localStorageService.GetItemAsStringAsync(StorageKey, cancellationToken);

		return tokenString;
	}

	public async Task StoreTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		await localStorageService.SetItemAsStringAsync(StorageKey, token, cancellationToken);

		TokenChanged?.Invoke(this, new(token));
	}

	public async Task DeleteTokenAsync(CancellationToken cancellationToken = default)
	{
		await localStorageService.RemoveItemAsync(StorageKey, cancellationToken);

		TokenChanged?.Invoke(this, new(null));
	}

	public event EventHandler<TokenChangedEventArgs>? TokenChanged;
}
