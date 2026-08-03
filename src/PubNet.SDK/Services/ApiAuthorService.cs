using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Authors;
using PubNet.SDK.Generated.Authors.Item;
using PubNet.SDK.Generated.Authors.Item.Delete;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class ApiAuthorService(PubNetApiClient apiClient, IAuthenticationService authenticationService)
	: IAuthorService
{
	public async Task<AuthorsResponseDto?> GetAuthorsAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Authors.GetAsync(cancellationToken: cancellationToken);
		}
		catch (AuthorsResponseDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<AuthorDto?> GetAuthorAsync(string username, CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Authors[username].GetAsync(cancellationToken: cancellationToken);
		}
		catch (AuthorNotFoundErrorDto)
		{
			return null;
		}
		catch (AuthorDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task UpdateAuthorAsync(string username, EditAuthorRequestDto request,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await apiClient.Authors[username].PatchAsync(request, cancellationToken: cancellationToken);
		}
		catch (AuthorDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ForbiddenErrorDto e)
		{
			throw new UnauthorizedException("You are not authorized to modify this author", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}

		// the modified author may be the authenticated one, whose model is cached separately
		authenticationService.InvalidateSelf();
	}

	public async Task DeleteAuthorAsync(string username, DeleteAuthorRequestDto request,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await apiClient.Authors[username].DeletePath.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (SuccessMessageDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ForbiddenErrorDto e)
		{
			throw new UnauthorizedException("You are not authorized to delete this author", e);
		}
		catch (InvalidPasswordErrorDto e)
		{
			throw new InvalidPasswordException(e);
		}
		catch (LastAdminErrorDto e)
		{
			throw new LastAdminException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}

		authenticationService.InvalidateSelf();
	}
}
