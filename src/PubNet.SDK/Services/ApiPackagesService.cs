using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Authors.Item.Packages;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Generated.Packages;
using PubNet.SDK.Generated.Packages.Item;
using PubNet.SDK.Generated.Packages.Item.Versions.Item;
using PubNet.SDK.Generated.Packages.Item.Versions.Item.Retract;

namespace PubNet.SDK.Services;

internal sealed class ApiPackagesService(PubNetApiClient apiClient) : IPackagesService
{
	public async Task<PackageDto?> GetPackageAsync(string name, bool includeAuthor,
		CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Packages[name]
				.GetAsync(r => r.QueryParameters.IncludeAuthor = includeAuthor, cancellationToken);
		}
		catch (PackageNotFoundErrorDto)
		{
			return null;
		}
		catch (PackageDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task DeletePackageAsync(string name, CancellationToken cancellationToken = default)
	{
		try
		{
			await apiClient.Packages[name].DeleteAsync(cancellationToken: cancellationToken);
		}
		catch (PackageNotFoundErrorDto e)
		{
			throw new PackageNotFoundException(name, e);
		}
		catch (WithName401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ForbiddenErrorDto e)
		{
			throw new UnauthorizedException("You are not authorized to delete this package", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task DeletePackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await apiClient.Packages[name].Versions[version].DeleteAsync(cancellationToken: cancellationToken);
		}
		catch (PackageVersionNotFoundErrorDto e)
		{
			throw new PackageVersionNotFoundException(name, version, e);
		}
		catch (WithVersion401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ForbiddenErrorDto e)
		{
			throw new UnauthorizedException("You are not authorized to delete this package version", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<PackageVersionDto?> GetPackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Packages[name].Versions[version].GetAsync(cancellationToken: cancellationToken);
		}
		catch (PackageVersionNotFoundErrorDto)
		{
			return null;
		}
		catch (PackageVersionDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task DiscontinuePackageAsync(string name, string? replacement,
		CancellationToken cancellationToken = default)
	{
		var dto = new SetDiscontinuedDto
		{
			Replacement = string.IsNullOrWhiteSpace(replacement) ? null : replacement,
		};

		try
		{
			_ = await apiClient.Packages[name].Discontinue.PatchAsync(dto, cancellationToken: cancellationToken);
		}
		catch (PackageNotFoundErrorDto e)
		{
			throw new PackageNotFoundException(name, e);
		}
		catch (PackageDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ForbiddenErrorDto e)
		{
			throw new UnauthorizedException("You are not authorized to discontinue this package", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task RetractPackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await apiClient.Packages[name].Versions[version].Retract.PatchAsync(cancellationToken: cancellationToken);
		}
		catch (PackageVersionNotFoundErrorDto e)
		{
			throw new PackageVersionNotFoundException(name, version, e);
		}
		catch (Retract401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ForbiddenErrorDto e)
		{
			throw new UnauthorizedException("You are not authorized to retract this package version", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<AuthorPackagesResponseDto?> GetPackagesByAuthorAsync(string username,
		CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Authors[username].Packages.GetAsync(cancellationToken: cancellationToken);
		}
		catch (AuthorNotFoundErrorDto)
		{
			return null;
		}
		catch (AuthorPackagesResponseDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<SearchPackagesResponseDto> SearchPackagesAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var response = await apiClient.Packages.GetAsync(cancellationToken: cancellationToken);

			return response ?? throw new UnexpectedResponseException("The response could not be deserialized");
		}
		catch (SearchPackagesResponseDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}
}
