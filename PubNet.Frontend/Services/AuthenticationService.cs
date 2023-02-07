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

	private AuthorDto? _self;
	private bool _fetchedSelf;

	public AuthenticationService(ILocalStorageService localStorage, ApiClient apiClient)
	{
		_localStorage = localStorage;
		_apiClient = apiClient;
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
	}

	public async Task<AuthorDto> GetSelfAsync(CancellationToken cancellationToken = default)
	{
		if (_apiClient.Token is null)
			throw new UnauthenticatedException("Not authenticated");

		if (_self is not null)
			return _self;

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

		return _self;
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
