using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PubNet.API.Abstractions.Archives;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.Services.Packages.Dart;

/// <inheritdoc />
public class DartPackageUploadService(IHttpContextAccessor contextAccessor, LinkGenerator linkGenerator, IBlobStorage blobStorage, IArchiveReader archiveReader) : IDartPackageUploadService
{
	/// <inheritdoc />
	public Task<DartArchiveUploadInformationDto> CreateNewAsync(Token token, CancellationToken cancellationToken = default)
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
	public async Task<DartPackageVersionDto> FinalizeNewAsync(DartPendingArchive pendingArchive, CancellationToken cancellationToken = default)
	{
		Stream archiveStream;

		var archiveUri = new Uri(pendingArchive.ArchivePath);
		switch (archiveUri.Scheme)
		{
			case "blob":
				var storageName = archiveUri.Host;
				var bucketName = archiveUri.Segments[1];
				var blobName = archiveUri.Segments[2];

				if (blobStorage.Name != storageName)
					throw new InvalidOperationException($"Blob storage {storageName} is not the expected storage {blobStorage.Name}");

				var blob = await blobStorage
					.GetBlob()
					.WithBucketName(bucketName)
					.WithBlobName(blobName)
					.RunAsync(cancellationToken);

				archiveStream = await blob.OpenReadAsync(cancellationToken);
				break;
			default:
				throw new NotSupportedException($"Unsupported archive URI scheme: {archiveUri.Scheme}");
		}

		string? pubspecYaml = null;
		await using (archiveStream)
		{
			foreach (var entry in archiveReader.EnumerateEntries(archiveStream))
			{
				if (entry is not { Name: "pubspec.yaml", IsDirectory: false })
					continue;

				await using var pubspecStream = entry.OpenRead();

				var pubspecReader = new StreamReader(pubspecStream);
				pubspecYaml = await pubspecReader.ReadToEndAsync(cancellationToken);

				break;
			}
		}

		if (pubspecYaml is null)
			throw new InvalidDartPackageException("Package does not contain a pubspec.yaml file or it could not be read");

		var pubspec = await ValidatePubspecAsync(pubspecYaml, cancellationToken);

		// TODO:
		// check if pending name matches name in pubspec
		// check if package entity exists, create if not

		var version = await ValidateVersionAsync(pubspec, cancellationToken);

		// TODO:
		// create package version entity
		// update latest version in package entity
		// delete pending archive

		throw new NotImplementedException();
	}

	private async Task<PubSpec> ValidatePubspecAsync(string pubspecYaml, CancellationToken cancellationToken = default)
	{
		// TODO:
		// parse yaml
		// check if required keys are present
		// check if certain values make sense (eg non-empty name, max length for fields)

		throw new NotImplementedException();
	}

	private async Task<string> ValidateVersionAsync(PubSpec pubspec, CancellationToken cancellationToken = default)
	{
		// TODO:
		// parse pubspec version
		// get latest version for package name
		// compare versions

		throw new NotImplementedException();
	}
}
