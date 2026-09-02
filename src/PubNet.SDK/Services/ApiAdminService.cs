using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Admin.Authors;
using PubNet.SDK.Generated.Admin.Authors.Item.Role;
using PubNet.SDK.Generated.Admin.Settings;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class ApiAdminService(PubNetApiClient apiClient, IAuthenticationService authenticationService)
	: IAdminService
{
	public async Task<List<SettingDescriptorDto>?> GetSettingsAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Admin.Settings.GetAsync(cancellationToken: cancellationToken);
		}
		catch (Settings401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task UpdateSettingsAsync(IReadOnlyDictionary<string, string?> settings,
		CancellationToken cancellationToken = default)
	{
		var request = new UpdateSettingsRequestDto
		{
			Settings = new()
			{
				AdditionalData = settings.ToDictionary(kv => kv.Key, object? (kv) => kv.Value)!,
			},
		};

		try
		{
			await apiClient.Admin.Settings.PatchAsync(request, cancellationToken: cancellationToken);
		}
		catch (Settings401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (UnknownSettingErrorDto e)
		{
			throw new UnknownSettingException(e.Error?.Message ?? "A key is not a configurable setting", e);
		}
		catch (InvalidSettingValueErrorDto e)
		{
			throw new InvalidSettingValueException(e.Error?.Message ?? "A setting value is invalid", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<AdminAuthorsResponseDto?> GetAuthorsAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Admin.Authors.GetAsync(cancellationToken: cancellationToken);
		}
		catch (AdminAuthorsResponseDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<AuthorDto?> SetAuthorRoleAsync(string username, Role role,
		CancellationToken cancellationToken = default)
	{
		var request = new SetAuthorRoleRequestDto
		{
			Role = role,
		};

		AuthorDto? author;
		try
		{
			author = await apiClient.Admin.Authors[username].Role
				.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (AuthorNotFoundErrorDto)
		{
			return null;
		}
		catch (AuthorDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (LastAdminErrorDto e)
		{
			throw new LastAdminException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}

		// the modified author may be the authenticated one, whose model is cached separately
		authenticationService.InvalidateSelf();

		return author;
	}
}
