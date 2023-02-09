using System.Net.Http.Json;
using Blazored.LocalStorage;
using PubNet.API.DTO;

namespace PubNet.Frontend.Services;

public class AuthenticationService
{
	private const string TokenStorageName = "authentication.token";
	private const string SelfStorageName = "authentication.self";

	private readonly ApiClient _apiClient;
	private readonly ILocalStorageService _localStorage;
	private readonly FetchLock<AuthenticationService> _fetchLock;

	private AuthorDto? _self;
	private bool _fetchedSelf;

	public event EventHandler? OnLogout;
	public event EventHandler? OnLogin;

	public AuthenticationService(ILocalStorageService localStorage, ApiClient apiClient, FetchLock<AuthenticationService> fetchLock)
	{
		_localStorage = localStorage;
		_apiClient = apiClient;
		_fetchLock = fetchLock;
	}

	public async ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
	{
		return _apiClient.Token ??= await _localStorage.GetItemAsync<string?>(TokenStorageName, cancellationToken);
	}

	public async ValueTask<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
	{
		return await GetTokenAsync(cancellationToken) is not null;
	}

	public async Task StoreTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		await _localStorage.SetItemAsync(TokenStorageName, token, cancellationToken);

		_apiClient.Token = token;
	}

	public async Task Logout(CancellationToken cancellationToken = default)
	{
		await RemoveTokenAsync(cancellationToken);
		await RemoveSelfAsync(cancellationToken);

		OnLogout?.Invoke(this, EventArgs.Empty);
	}

	private async Task RemoveTokenAsync(CancellationToken cancellationToken = default)
	{
		await _localStorage.RemoveItemAsync(TokenStorageName, cancellationToken);

		_apiClient.Token = null;
	}

	private async Task RemoveSelfAsync(CancellationToken cancellationToken = default)
	{
		await _localStorage.RemoveItemAsync(SelfStorageName, cancellationToken);

		_self = null;
		_fetchedSelf = false;
	}

	public async Task<AuthorDto> GetSelfAsync(CancellationToken cancellationToken = default)
	{
		if (_apiClient.Token is null)
			throw new UnauthenticatedException("Not authenticated");

		await _fetchLock.UntilFreed();

		if (_self is not null)
			return _self;

		_fetchLock.Lock();
		try
		{
			var storedSelf = await _localStorage.GetItemAsync<AuthorDto>(SelfStorageName, cancellationToken);
			if (_fetchedSelf && storedSelf is not null)
			{
				_self = storedSelf;

				return _self;
			}

			var response = await _apiClient.GetAsync("authentication/self", cancellationToken);
			if (!response.IsSuccessStatusCode)
				throw new UnauthenticatedException("Request failed");

			_self = await response.Content.ReadFromJsonAsync<AuthorDto>(cancellationToken: cancellationToken);
			if (_self is null)
				throw new UnauthenticatedException("Unable to deserialize");

			await _localStorage.SetItemAsync(SelfStorageName, _self, cancellationToken);

			_fetchedSelf = true;

			OnLogin?.Invoke(this, EventArgs.Empty);

			return _self;
		}
		finally
		{
			_fetchLock.Free();
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

public class UnauthenticatedException : Exception
{
	public UnauthenticatedException(string message) : base(message)
	{
	}
}
