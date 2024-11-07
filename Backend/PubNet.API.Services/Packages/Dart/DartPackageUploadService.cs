using System.Diagnostics;
using DartLang.PubSpec;
using DartLang.PubSpec.Serialization.Yaml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PubNet.API.Abstractions.Archives;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;
using PubNet.PackageStorage.Abstractions;
using Semver;

namespace PubNet.API.Services.Packages.Dart;

/// <inheritdoc />
public class DartPackageUploadService(
	IHttpContextAccessor contextAccessor,
	LinkGenerator linkGenerator,
	IBlobStorage blobStorage,
	IArchiveReader archiveReader,
	IDartPackageDao packageDao,
	IDartPackageDmo packageDmo,
	IArchiveStorage archiveStorage) : IDartPackageUploadService
{
	/// <inheritdoc />
	public Task<DartArchiveUploadInformationDto> CreateNewAsync(Token token,
		CancellationToken cancellationToken = default)
	{
		var url = GetUriForUploadEndpoint();
		var fields = new Dictionary<string, string>
		{
			{ "author-id", token.Identity.Author.Id.ToString() },
		};

		Debug.Assert(url.IsAbsoluteUri, "URL must be absolute to ensure it is not tied to a specific host");

		var dto = new DartArchiveUploadInformationDto
		{
			Url = url.ToString(),
			Fields = fields,
		};

		return Task.FromResult(dto);
	}

	private Uri GetUriForUploadEndpoint()
	{
		var path = linkGenerator.GetPathByAction("Upload", "DartStorage");
		if (path is null)
			throw new InvalidOperationException("Could not generate URL for upload endpoint");

		var context = contextAccessor.HttpContext;
		if (context is null)
			throw new InvalidOperationException("Could not determine host for upload endpoint");

		var builder = new UriBuilder
		{
			Scheme = context.Request.Scheme,
			Host = context.Request.Host.Host,
			Port = context.Request.Host.Port ?? 80,
			Path = path,
		};

		return builder.Uri;
	}

	/// <inheritdoc />
	public async Task<SemVersion> FinalizeNewAsync(DartPendingArchive pendingArchive,
		CancellationToken cancellationToken = default)
	{
		var (archiveMemoryStream, deletePendingArchive) = await ReadArchiveStream(pendingArchive, cancellationToken);

		PubSpec pubspec;
		DartPackage? package;
		SemVersion version;
		await using (archiveMemoryStream)
		{
			pubspec = await ValidatePubspecAsync(archiveMemoryStream, cancellationToken);

			package = await packageDao.GetByNameAsync(pubspec.Name, cancellationToken);
			if (package is not null)
			{
				if (package.Author.Id != pendingArchive.Uploader.Id)
					throw new UnauthorizedAccessException(
						"You are not authorized to upload new versions for this package");
			}

			version = ValidateVersion(package, pubspec);

			Debug.Assert(archiveMemoryStream.CanSeek);

			archiveMemoryStream.Seek(0, SeekOrigin.Begin);

			_ = await archiveStorage.StoreArchiveAsync(pendingArchive.Uploader.UserName, pubspec.Name,
				version.ToString(),
				archiveMemoryStream, cancellationToken);
		}

		if (package is not null)
		{
			var versionEntity = new DartPackageVersion
			{
				Package = package,
				Version = version.ToString(),
				PublishedAt = DateTimeOffset.UtcNow,
				PubSpec = pubspec,
			};

			await packageDmo.SaveLatestVersionAsync(package, versionEntity, cancellationToken);
		}
		else
			_ = await packageDmo.CreateAsync(pubspec.Name, pendingArchive.Uploader, version, pubspec, cancellationToken);

		await deletePendingArchive();

		return version;
	}

	private async Task<PubSpec> ReadPubspecYamlAsync(Stream archiveStream,
		CancellationToken cancellationToken)
	{
		foreach (var entry in archiveReader.EnumerateEntries(archiveStream, leaveStreamOpen: true))
		{
			if (entry is not { Name: "pubspec.yaml", IsDirectory: false })
				continue;

			await using var pubspecStream = entry.OpenRead();

			return await PubSpecYamlConverter.DeserializeAsync(pubspecStream, cancellationToken);
		}

		throw new InvalidDartPackageException("Package does not contain a pubspec.yaml file or it could not be read");
	}

	private async Task<(Stream archive, Func<Task> deleteAction)> ReadArchiveStream(
		DartPendingArchive pendingArchive,
		CancellationToken cancellationToken)
	{
		Stream archiveStream;
		Func<Task> deleteAction;

		var archiveUri = new Uri(pendingArchive.ArchivePath);
		switch (archiveUri.Scheme)
		{
			case "blob":
				var storageName = archiveUri.Host;
				var bucketName = archiveUri.Segments[1];
				var blobName = archiveUri.Segments[2];

				if (!string.Equals(blobStorage.Name, storageName, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException(
						$"Blob storage {storageName} is not the expected storage {blobStorage.Name}");

				var blob = await blobStorage
					.GetBlob()
					.WithBucketName(bucketName)
					.WithBlobName(blobName)
					.RunAsync(cancellationToken);

				archiveStream = await blob.OpenReadAsync(cancellationToken);

				deleteAction = async () => await blobStorage
					.DeleteBlob()
					.WithBucketName(bucketName)
					.WithBlobName(blobName)
					.RunAsync(cancellationToken);
				break;
			default:
				throw new NotSupportedException($"Unsupported archive URI scheme: {archiveUri.Scheme}");
		}

		return (archiveStream, deleteAction);
	}

	private async Task<PubSpec> ValidatePubspecAsync(Stream archiveMemoryStream,
		CancellationToken cancellationToken)
	{
		var pubspec = await ReadPubspecYamlAsync(archiveMemoryStream, cancellationToken);

		// TODO: check if required keys are present
		// TODO: check if certain values make sense (eg non-empty name, max length for fields)
		// TODO: check if publish_to matches this host

		return pubspec;
	}

	private static SemVersion ValidateVersion(DartPackage? package, PubSpec pubspec)
	{
		if (pubspec.Version is not { } version)
			throw new InvalidDartPackageException("pubspec.yaml does not contain a valid SemVer version");

		if (package is null)
			return version; // no latest version to compare against

		var latestVersionEntity = package.LatestVersion;
		if (latestVersionEntity is null)
			return version;

		var latestVersion = SemVersion.Parse(latestVersionEntity.Version, SemVersionStyles.Strict);

		switch (version.CompareSortOrderTo(latestVersion))
		{
			case 0:
				throw new DartPackageVersionAlreadyExistsException(pubspec.Name, version.ToString());
			case > 0:
				throw new DartPackageVersionOutdatedException(pubspec.Name, version.ToString());
		}

		return version;
	}
}
