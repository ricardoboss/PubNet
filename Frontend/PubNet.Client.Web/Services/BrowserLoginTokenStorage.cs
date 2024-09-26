using Blazored.LocalStorage;
using PubNet.Client.Abstractions;
using PubNet.Auth.Models;

namespace PubNet.Client.Web.Services;

public class BrowserLoginTokenStorage(ILocalStorageService localStorageService) : ILoginTokenStorage
{
	private const string StorageKey = "loginToken";

	public async Task<JsonWebToken?> GetTokenAsync(CancellationToken cancellationToken = default)
	{
		var tokenString = await localStorageService.GetItemAsStringAsync(StorageKey, cancellationToken);
		if (tokenString is null)
			return null;

		return JsonWebToken.From(tokenString);
	}

	public async Task StoreTokenAsync(JsonWebToken token, CancellationToken cancellationToken = default)
	{
		await localStorageService.SetItemAsStringAsync(StorageKey, token.Value, cancellationToken);

		TokenChanged?.Invoke(this, new(token));
	}

	public async Task DeleteTokenAsync(CancellationToken cancellationToken = default)
	{
		await localStorageService.RemoveItemAsync(StorageKey, cancellationToken);

		TokenChanged?.Invoke(this, new(null));
	}

	public event EventHandler<TokenChangedEventArgs>? TokenChanged;
}
