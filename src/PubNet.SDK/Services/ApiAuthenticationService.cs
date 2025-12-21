using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Authentication.Self;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class ApiAuthenticationService(
	PubNetApiClient apiClient,
	ILoginTokenStorage loginTokenStorage
) : IAuthenticationService
{
	private AuthorDto? _self;

	public async Task<bool> GetRegistrationsEnabledAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Authentication.RegistrationsEnabled.GetAsync(cancellationToken: cancellationToken) ??
				throw new UnexpectedResponseException("Unable to deserialize registrations enabled response");
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			return await loginTokenStorage.GetTokenAsync(cancellationToken) is not null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public async Task<JsonWebTokenResponseDto> LoginAsync(string email, string password,
		CancellationToken cancellationToken = default)
	{
		var request = new LoginRequestDto
		{
			Email = email,
			Password = password,
		};

		try
		{
			var response =
				await apiClient.Authentication.Login.PostAsync(request, cancellationToken: cancellationToken);

			return response ?? throw new UnexpectedResponseException("Unable to deserialize login response");
		}
		catch (EmailNotFoundErrorDto e)
		{
			throw new InvalidLoginCredentialsException($"E-mail not found: {email}", e);
		}
		catch (InvalidPasswordErrorDto e)
		{
			throw new InvalidLoginCredentialsException("Wrong password", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task LogoutAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			await loginTokenStorage.DeleteTokenAsync(cancellationToken);
		}
		finally
		{
			_self = null;
		}
	}

	public async Task<AuthorDto> GetSelfAsync(bool forceLoad = false, CancellationToken cancellationToken = default)
	{
		if (!await IsAuthenticatedAsync(cancellationToken))
			throw new AuthenticationRequiredException("Not authenticated");

		if (!forceLoad && _self is not null)
			return _self;

		try
		{
			var maybeAuthor = await apiClient.Authentication.Self.GetAsync(cancellationToken: cancellationToken);

			return _self = maybeAuthor ??
				throw new UnexpectedResponseException("Failed to deserialize self response");
		}
		catch (AuthorDto401Error e)
		{
			// token probably expired
			await LogoutAsync(CancellationToken.None);

			throw new AuthenticationRequiredException("Not authenticated", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<bool> IsSelfAsync(string username, CancellationToken cancellationToken = default)
	{
		try
		{
			var self = await GetSelfAsync(cancellationToken: cancellationToken);

			return self.UserName == username;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
