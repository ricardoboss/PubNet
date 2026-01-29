using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

/// <summary>
/// Provides package and package version data and allows to modify packages and package versions.
/// </summary>
public interface IPackagesService
{
	/// <summary>
	/// Gets a single package by its name and optionally includes information about the author.
	/// </summary>
	/// <param name="name">The name of the package to retrieve</param>
	/// <param name="includeAuthor">Whether to include the author of the package</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A <see cref="PackageDto"/>, optionally including the author, or <see langword="null"/> if the package could not be found</returns>
	/// <exception cref="AuthenticationRequiredException">If the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<PackageDto?> GetPackageAsync(string name, bool includeAuthor, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deletes a package and all its versions.
	/// </summary>
	/// <param name="name">The name of the package to delete</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A task representing the asynchronous request</returns>
	/// <exception cref="PackageNotFoundException">If no package with the given <paramref name="name"/> could be found</exception>
	/// <exception cref="AuthenticationRequiredException">If the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="UnauthorizedException">If the given credentials don't have the required permissions</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task DeletePackageAsync(string name, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deletes a specific package version.
	/// </summary>
	/// <param name="name">The name of the package</param>
	/// <param name="version">The version to delete</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A task representing the asynchronous request</returns>
	/// <exception cref="PackageVersionNotFoundException">If either the package with the given <paramref name="name"/> wasn't found or this specific package <paramref name="version"/> could not be found</exception>
	/// <exception cref="AuthenticationRequiredException">If the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="UnauthorizedException">If the given credentials don't have the required permissions</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task DeletePackageVersionAsync(string name, string version, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets a specific package version.
	/// </summary>
	/// <param name="name">The name of the package</param>
	/// <param name="version">The version to retrieve</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A <see cref="PackageVersionDto"/>, or <see langword="null"/> if the package or the specific package version could not be found</returns>
	/// <exception cref="AuthenticationRequiredException">If the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<PackageVersionDto?> GetPackageVersionAsync(string name, string version,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Marks a package as "discontinued", optionally with a package to advertise as a replacement.
	/// </summary>
	/// <param name="name">The name of the package to discontinue</param>
	/// <param name="replacement">Optionally, a package to advertise as the replacement</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A task representing the asynchronous request</returns>
	/// <exception cref="PackageNotFoundException">If no package with the given <paramref name="name"/> could be found</exception>
	/// <exception cref="AuthenticationRequiredException">If the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="UnauthorizedException">If the given credentials don't have the required permissions</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task DiscontinuePackageAsync(string name, string? replacement, CancellationToken cancellationToken = default);

	/// <summary>
	/// Retracts a specific package version, effectively unpublishing it.
	/// </summary>
	/// <param name="name">The name of the package</param>
	/// <param name="version">The version to retract</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous task</param>
	/// <returns>A task representing the asynchronous request</returns>
	/// <exception cref="PackageVersionNotFoundException">If either the package with the given <paramref name="name"/> wasn't found or this specific package <paramref name="version"/> could not be found</exception>
	/// <exception cref="AuthenticationRequiredException">When the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="UnauthorizedException">If the given credentials don't have the required permissions</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task RetractPackageVersionAsync(string name, string version, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets a list of packages from a specific author.
	/// </summary>
	/// <param name="username">The username of the author</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A <see cref="AuthorPackagesResponseDto"/>, or <see langword="null"/> if the author could not be found</returns>
	/// <exception cref="AuthenticationRequiredException">If the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<AuthorPackagesResponseDto?> GetPackagesByAuthorAsync(string username,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets a list of all available packages.
	/// </summary>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A <see cref="SearchPackagesResponseDto"/></returns>
	/// <exception cref="AuthenticationRequiredException">If the data source requires authentication, but no credentials were provided</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<SearchPackagesResponseDto> SearchPackagesAsync(CancellationToken cancellationToken = default);
}
