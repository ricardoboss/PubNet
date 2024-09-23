using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.Abstractions.Packages.Dart;

/// <summary>
/// Responsible for handling the upload of new package versions.
/// </summary>
public interface IDartPackageUploadService
{
	/// <summary>
	/// Use the given token to create a new (preliminary) package version.
	/// </summary>
	/// <param name="token">The token that was used in the upload request.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A <see cref="DartArchiveUploadInformationDto"/> that contains the information needed to upload the package.</returns>
	Task<DartArchiveUploadInformationDto> CreateNewAsync(Token token, CancellationToken cancellationToken = default);

	/// <summary>
	/// Finalizes a new package version by unpacking the package archive, verifying its contents and creating the
	/// necessary entities.
	/// </summary>
	/// <param name="pendingArchive">The pending archive to finalize.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A <see cref="DartPackageVersionDto"/> representing the finalized package version.</returns>
	/// <exception cref="DartPackageVersionAlreadyExistsException">Thrown when the package version already exists.</exception>
	/// <exception cref="DartPackageVersionOutdatedException">Thrown when the package version is older than the latest version.</exception>
	/// <exception cref="InvalidDartPackageException">Thrown when the package is invalid.</exception>
	Task<DartPackageVersionDto> FinalizeNewAsync(DartPendingArchive pendingArchive, CancellationToken cancellationToken = default);
}
