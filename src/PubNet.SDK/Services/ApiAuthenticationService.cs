using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

public class ApiAuthenticationService(
	PubNetApiClient apiClient,
	ILoginTokenStorage loginTokenStorage
) : IAuthenticationService
{
	private AuthorDto? _self;

	public async Task<bool> GetRegistrationsEnabledAsync(CancellationToken cancellationToken = default)
	{
		return await apiClient.Authentication.RegistrationsEnabled.GetAsync(cancellationToken: cancellationToken) ??
			false;
	}

	public async ValueTask<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
	{
		return await loginTokenStorage.GetTokenAsync(cancellationToken) is not null;
	}

	public async Task<JsonWebTokenResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
	{
		return await apiClient.Authentication.Login.PostAsync(request, cancellationToken: cancellationToken);
	}

	public async Task LogoutAsync(CancellationToken cancellationToken = default)
	{
		await loginTokenStorage.DeleteTokenAsync(cancellationToken);

		_self = null;
	}

	public async Task<AuthorDto?> GetSelfAsync(CancellationToken cancellationToken = default)
	{
		if (!await IsAuthenticatedAsync(cancellationToken))
			throw new UnauthenticatedException("Not authenticated");

		if (_self is not null)
			return _self;

		var maybeAuthor = await apiClient.Authentication.Self.GetAsync(cancellationToken: cancellationToken);

		return _self = maybeAuthor ?? throw new UnauthenticatedException("Request failed");
	}

	public async Task<bool> IsSelfAsync(string? username, CancellationToken cancellationToken = default)
	{
		if (username is null) return false;

		try
		{
			var self = await GetSelfAsync(cancellationToken);

			return self?.UserName == username;
		}
		catch (Exception e)
		{
			if (e is UnauthenticatedException)
				await LogoutAsync(CancellationToken.None);

			return false;
		}
	}
}
