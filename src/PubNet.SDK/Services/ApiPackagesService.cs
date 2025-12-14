using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Helpers;

namespace PubNet.SDK.Services;

public class ApiPackagesService(PubNetApiClient apiClient, FetchLock<ApiPackagesService> fetchLock) :
	IPackagesService
{
	private readonly Dictionary<string, PackageDto?> _packages = new();

	private static UnauthorizedAccessException Unauthorized(string message) => new(message);
	private static PackageNotFoundException NotFound(string? message = null) => new(message ?? "The package you are looking for does not exist");
	private static PackageFetchException Undocumented => new("An undocumented error occurred");

	public async Task<PackageDto?> GetPackageAsync(string name, CancellationToken cancellationToken = default)
	{
		await fetchLock.UntilFreed(taskName: $"GetPackage({name})");

		using var _ = fetchLock.Lock($"GetPackage({name})");

		try
		{
			var package = await apiClient.Packages[name].GetAsync(cancellationToken: cancellationToken);

			_packages[name] = package;

			return package;
		}
		catch (PackageNotFoundErrorDto)
		{
			throw NotFound();
		}
		catch (Exception)
		{
			throw Undocumented;
		}
	}

	public async Task DeletePackageAsync(string name, CancellationToken cancellationToken = default)
	{
		using var _ = await fetchLock.UntilFreedAndLock(taskName: $"DeletePackage({name})");

		try
		{
			await apiClient.Packages[name].DeleteAsync(cancellationToken: cancellationToken);
		}
		catch (PackageNotFoundErrorDto)
		{
			throw NotFound("The package you are trying to delete was not found");
		}
		catch (ForbiddenErrorDto)
		{
			throw Unauthorized("You are not authorized to delete this package");
		}
		catch (Exception)
		{
			throw Undocumented;
		}
		finally
		{
			// delete package from cache, regardless of API call result, so it has to be re-fetched
			_packages.Remove(name);
		}
	}

	public async Task DeletePackageVersionAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		using var _ = await fetchLock.UntilFreedAndLock(taskName: $"DeletePackageVersion({name}, {version})");

		try
		{
			await apiClient.Packages[name].Versions[version].DeleteAsync(cancellationToken: cancellationToken);
		}
		catch (PackageNotFoundErrorDto)
		{
			throw NotFound("The package version you are trying to delete was not found");
		}
		catch (ForbiddenErrorDto)
		{
			throw Unauthorized("You are not authorized to delete this package version");
		}
		catch (Exception)
		{
			throw Undocumented;
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package) && package?.Versions != null)
			{
				var versionIdx = package.Versions.FindIndex(v => v.Version == version);

				package.Versions.RemoveAt(versionIdx);
			}
		}
	}

	public async Task<PackageVersionDto?> GetPackageVersionAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		return (await GetPackageAsync(name, cancellationToken))?.Versions?.FirstOrDefault(v => v.Version == version) ?? throw NotFound("The package version you are looking for does not exist");
	}

	public async Task DiscontinuePackageAsync(string name, string? replacement, CancellationToken cancellationToken = default)
	{
		using var __ = await fetchLock.UntilFreedAndLock(taskName: $"DiscontinuePackage({name}, {replacement})");

		try
		{
			var dto = new SetDiscontinuedDto
			{
				Replacement = string.IsNullOrWhiteSpace(replacement) ? null : replacement,
			};

			_ = await apiClient.Packages[name].Discontinue.PatchAsync(dto, cancellationToken: cancellationToken);
		}
		catch (PackageNotFoundErrorDto)
		{
			throw NotFound();
		}
		catch (ForbiddenErrorDto)
		{
			throw Unauthorized("You are not authorized to discontinue this package");
		}
		catch (Exception)
		{
			throw Undocumented;
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package) && package is not null)
			{
				package.IsDiscontinued = true;
			}
		}
	}

	public async Task RetractPackageVersionAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		using var _ = await fetchLock.UntilFreedAndLock(taskName: $"RetractVersion({name}, {version})");
		try
		{
			await apiClient.Packages[name].Versions[version].Retract.PatchAsync(cancellationToken: cancellationToken);
		}
		catch (PackageNotFoundErrorDto)
		{
			throw NotFound("The package version you are trying to retract does not exist");
		}
		catch (ForbiddenErrorDto)
		{
			throw Unauthorized("You are not authorized to retract this package version");
		}
		catch (Exception)
		{
			throw Undocumented;
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package))
			{
				var dto = package?.Versions?.FirstOrDefault(v => v.Version == version);
				dto?.Retracted = true;
			}
		}
	}

	public async Task<AuthorPackagesResponseDto?> GetPackagesByAuthorAsync(string username, CancellationToken cancellationToken = default)
	{
		return await apiClient.Authors[username].Packages.GetAsync(cancellationToken: cancellationToken);
	}

	public async Task<SearchPackagesResponseDto?> SearchPackagesAsync(CancellationToken cancellationToken = default)
	{
		return await apiClient.Packages.GetAsync(cancellationToken: cancellationToken);
	}
}
