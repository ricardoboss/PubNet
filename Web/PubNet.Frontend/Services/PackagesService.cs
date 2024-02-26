using PubNet.Client.Generated;
using PubNet.Client.Generated.Models;

namespace PubNet.Frontend.Services;

public class PackagesService(ApiClient http, AnalysisService analysis, FetchLock<PackagesService> fetchLock)
{
	private readonly Dictionary<string, DartPackageDto?> _packages = new Dictionary<string, DartPackageDto?>();

	private static PackageNotFoundException NotFound(string? message = null) => new(message ?? "The package you are looking for does not exist");
	public async Task<DartPackageDto?> GetPackage(string name, bool includeAuthor, bool forceFetch = false, CancellationToken cancellationToken = default)
	{
		await fetchLock.UntilFreed(taskName: $"GetPackage({name}, {includeAuthor})");

		if (!forceFetch && _packages.TryGetValue(name, out var package))
			return package ?? throw NotFound();

		fetchLock.Lock($"GetPackage({name}, {includeAuthor})");
		try
		{
			var fetchedPackage = await http.Packages.Dart[name].GetAsync(cancellationToken: cancellationToken);

			_packages[name] = fetchedPackage;

			return fetchedPackage;
		}
		finally
		{
			fetchLock.Free();
		}
	}

	// public async Task DeletePackage(string name, CancellationToken cancellationToken = default)
	// {
	// 	await fetchLock.LockAfterFreed(taskName: $"DeletePackage({name})");
	// 	try
	// 	{
	// 		// var response = await http.DeleteAsync($"packages/{name}", cancellationToken);
	// 		// if (response.IsSuccessStatusCode) return;
	// 		//
	// 		// throw response.StatusCode switch
	// 		// {
	// 		// 	HttpStatusCode.Unauthorized => Unauthorized("You are not authorized to delete this package"),
	// 		// 	HttpStatusCode.NotFound => NotFound("The package you are trying to delete was not found"),
	// 		// 	HttpStatusCode.InternalServerError => InternalServerError,
	// 		// 	_ => Undocumented,
	// 		// };
	// 		// TODO(rbo): check if this is the correct way to delete a package
	// 		await http.Packages.Dart[name].Discontinue.PatchAsync(cancellationToken: cancellationToken);
	// 	}
	// 	finally
	// 	{
	// 		// delete package from cache, regardless of API call result, so it has to be re-fetched
	// 		_packages.Remove(name);
	//
	// 		analysis.InvalidateAnalysisFor(name);
	//
	// 		fetchLock.Free();
	// 	}
	// }

	// public async Task DeletePackageVersion(string name, string version, CancellationToken cancellationToken = default)
	// {
	// 	await fetchLock.LockAfterFreed(taskName: $"DeletePackageVersion({name}, {version})");
	// 	try
	// 	{
	// 		// var response = await http.DeleteAsync($"packages/{name}/versions/{version}", cancellationToken);
	// 		// if (response.IsSuccessStatusCode) return;
	// 		//
	// 		// throw response.StatusCode switch
	// 		// {
	// 		// 	HttpStatusCode.Unauthorized => Unauthorized("You are not authorized to delete this package version"),
	// 		// 	HttpStatusCode.NotFound => NotFound("The package version you are trying to delete was not found"),
	// 		// 	HttpStatusCode.InternalServerError => InternalServerError,
	// 		// 	_ => Undocumented,
	// 		// };
	// 		// TODO(rbo): check if this is the correct way to delete a package version
	// 		await http.Packages.Dart[name][version].Retract.PatchAsync(cancellationToken: cancellationToken);
	// 	}
	// 	finally
	// 	{
	// 		if (_packages.TryGetValue(name, out var package) && package?.Versions != null)
	// 		{
	// 			var versionIdx = package.Versions.FindIndex(v => v.Version == version);
	//
	// 			package.Versions.RemoveAt(versionIdx);
	// 		}
	//
	// 		analysis.InvalidateAnalysisFor(name, version);
	//
	// 		fetchLock.Free();
	// 	}
	// }

	public async Task<DartPackageVersionDto?> GetPackageVersion(string name, string version)
	{
		return (await GetPackage(name, false))?.Versions?.FirstOrDefault(v => v.Version == version) ?? throw NotFound("The package version you are looking for does not exist");
	}

	public async Task DiscontinuePackage(string name, string? replacement, CancellationToken cancellationToken = default)
	{
		await fetchLock.LockAfterFreed(taskName: $"DiscontinuePackage({name}, {replacement})");
		try
		{
			await http.Packages.Dart[name].Discontinue.PatchAsync(cancellationToken: cancellationToken);
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package) && package is not null)
			{
				package.IsDiscontinued = true;
			}

			analysis.InvalidateAnalysisFor(name);

			fetchLock.Free();
		}
	}

	public async Task RetractVersion(string name, string version, CancellationToken cancellationToken = default)
	{
		await fetchLock.LockAfterFreed(taskName: $"RetractVersion({name}, {version})");
		try
		{
			await http.Packages.Dart[name][version].Retract.PatchAsync(cancellationToken: cancellationToken);
		}
		finally
		{
			if (_packages.TryGetValue(name, out var package))
			{
				var dto = package?.Versions?.FirstOrDefault(v => v.Version == version);
				if (dto is not null) dto.Retracted = true;
			}

			analysis.InvalidateAnalysisFor(name, version);

			fetchLock.Free();
		}
	}
}
