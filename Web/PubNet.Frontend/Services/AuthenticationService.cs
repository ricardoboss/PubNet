using Blazored.LocalStorage;
using PubNet.Client.Generated;
using PubNet.Client.Generated.Models;

namespace PubNet.Frontend.Services;

public class AuthenticationService(
	ILocalStorageService localStorage,
	ApiClient apiClient,
	FetchLock<AuthenticationService> fetchLock)
{
	private const string TokenStorageName = "authentication.token";
	private const string SelfStorageName = "authentication.self";

	private AuthorDto? _self;
	private bool _fetchedSelf;

	public ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
	{
		return localStorage.GetItemAsync<string?>(TokenStorageName, cancellationToken);
	}

	public async ValueTask<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
	{
		return await GetTokenAsync(cancellationToken) is not null;
	}

	public async Task StoreTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		await localStorage.SetItemAsync(TokenStorageName, token, cancellationToken);
	}

	public async Task Logout(CancellationToken cancellationToken = default)
	{
		await RemoveTokenAsync(cancellationToken);
		await RemoveSelfAsync(cancellationToken);
	}

	private async Task RemoveTokenAsync(CancellationToken cancellationToken = default)
	{
		await localStorage.RemoveItemAsync(TokenStorageName, cancellationToken);
	}

	private async Task RemoveSelfAsync(CancellationToken cancellationToken = default)
	{
		await localStorage.RemoveItemAsync(SelfStorageName, cancellationToken);

		_self = null;
		_fetchedSelf = false;
	}

	public async Task<AuthorDto> GetSelfAsync(CancellationToken cancellationToken = default)
	{
		await fetchLock.UntilFreed();

		if (_self is not null)
			return _self;

		fetchLock.Lock();
		try
		{
			var storedSelf = await localStorage.GetItemAsync<AuthorDto>(SelfStorageName, cancellationToken);
			if (_fetchedSelf && storedSelf is not null)
			{
				_self = storedSelf;

				return _self;
			}

			_self = await apiClient.Authentication.Self.GetAsync(cancellationToken: cancellationToken);
			if (_self is null)
				throw new UnauthenticatedException("Failed to fetch self");

			await localStorage.SetItemAsync(SelfStorageName, _self, cancellationToken);

			_fetchedSelf = true;

			return _self;
		}
		finally
		{
			fetchLock.Free();
		}
	}

	public async Task<bool> IsSelf(string? username, CancellationToken cancellationToken = default)
	{
		if (username is null) return false;

		try
		{
			return (await GetSelfAsync(cancellationToken)).UserName == username;
		}
		catch (Exception e)
		{
			if (e is UnauthenticatedException)
				await Logout(CancellationToken.None);

			return false;
		}
	}
}

public class UnauthenticatedException(string message) : Exception(message);
