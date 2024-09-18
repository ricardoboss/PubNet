using Blazored.LocalStorage;
using Microsoft.Kiota.Abstractions.Authentication;
using PubNet.Frontend.Interfaces;

namespace PubNet.Frontend.Services;

public class LocalAccessTokenStorage(ILocalStorageService localStorage) : IAccessTokenStorage
{
	private const string TokenStorageName = "authentication.token";

	public ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
	{
		return localStorage.GetItemAsync<string?>(TokenStorageName, cancellationToken);
	}

	public async Task StoreTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		await localStorage.SetItemAsync(TokenStorageName, token, cancellationToken);
	}

	public async Task RemoveTokenAsync(CancellationToken cancellationToken = default)
	{
		await localStorage.RemoveItemAsync(TokenStorageName, cancellationToken);
	}

	public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null,
		CancellationToken cancellationToken = default)
	{
		return await GetTokenAsync(cancellationToken) ?? string.Empty;
	}

	public AllowedHostsValidator AllowedHostsValidator { get; } = new()
	{
		AllowedHosts =
		[
			"https://localhost:7171",
		],
	};
}
