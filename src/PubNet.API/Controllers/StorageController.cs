using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.DTO.Packages;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Common.Interfaces;
using PubNet.Common.Models;
using PubNet.Common.Services;
using PubNet.Common.Utils;
using PubNet.Database;
using PubNet.Database.Models;
using Semver;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PubNet.API.Controllers;

[ApiController]
[Route("storage")]
public class StorageController(
	ILogger<StorageController> logger,
	PubNetContext db,
	IPackageStorageProvider storageProvider,
	EndpointHelper endpointHelper
) : BaseController, IUploadEndpointGenerator
{
	/// <inheritdoc />
	[NonAction]
	public Task<UploadEndpointData> GenerateUploadEndpointData(HttpRequest request, Author author,
		CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var url = endpointHelper.GenerateFullyQualified(request, "/api/storage/upload", new Dictionary<string, string?>
		{
			{ "authorId", author.Id.ToString() },
		});

		url = endpointHelper.SignEndpoint(url);

		return Task.FromResult(new UploadEndpointData(url, new()));
	}

	[HttpPost("upload")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status411LengthRequired, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status413PayloadTooLarge, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Upload([FromQuery] int? authorId)
	{
		if (!endpointHelper.ValidateSignature(Request.QueryString.ToString()))
			return BadRequest(ErrorResponse.InvalidSignedUrl);

		const long maxUploadSize = 100_000_000; // 100 MB

		var size = Request.Headers.ContentLength;
		switch (size)
		{
			case null:
				return StatusCode(StatusCodes.Status411LengthRequired, ErrorResponse.PackageLengthRequired);
			case > maxUploadSize:
				return StatusCode(StatusCodes.Status413PayloadTooLarge,
					ErrorResponse.PackagePayloadTooLarge(maxUploadSize));
		}

		var packageFile = Request.Form.Files.FirstOrDefault(f => f.Name == "file");
		if (packageFile is null)
			return BadRequest(ErrorResponse.MissingPackageFile);

		if (authorId is null)
			return BadRequest(ErrorResponse.MissingFields);

		var author = await db.Authors.FindAsync(authorId);
		if (author is null)
			return BadRequest(ErrorResponse.InvalidAuthorId);

		var pendingId = Guid.NewGuid();
		var tempFile = Path.GetTempPath() + pendingId + ".tar.gz";

		logger.LogDebug("Storing package archive for {Author} at {Path}", author.UserName, tempFile);

		await using (var fileStream = System.IO.File.OpenWrite(tempFile))
		{
			await packageFile.CopyToAsync(fileStream);
		}

		var pending = new PendingArchive
		{
			Uuid = pendingId,
			ArchivePath = tempFile,
			Uploader = author,
			UploadedAtUtc = DateTimeOffset.UtcNow,
		};

		db.PendingArchives.Add(pending);
		await db.SaveChangesAsync();

		logger.LogTrace("Added pending archive {ArchiveId}", pending.Uuid);

		var finalizeUrl = endpointHelper.SignEndpoint(
			endpointHelper.GenerateFullyQualified(
				Request,
				"/api/storage/finalize",
				new Dictionary<string, string?>
				{
					{ "pendingId", pendingId.ToString() },
				}
			)
		);

		logger.LogTrace("Generated finalize url for archive {ArchiveId} to {FinalizeUrl}", pending.Uuid, finalizeUrl);

		Response.Headers.Location = finalizeUrl;
		return NoContent();
	}

	[HttpGet("finalize")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> FinalizeUpload([FromQuery] string? pendingId,
		CancellationToken cancellationToken = default)
	{
		if (!endpointHelper.ValidateSignature(Request.QueryString.ToString()))
			return BadRequest(ErrorResponse.InvalidSignedUrl);

		if (pendingId is null)
			return FailedDependency(ErrorResponse.MissingPendingId);

		if (!Guid.TryParse(pendingId, out var uuid))
			return FailedDependency(ErrorResponse.InvalidPendingId);

		var pending = await db.PendingArchives
			.Include(p => p.Uploader)
			.FirstOrDefaultAsync(a => a.Uuid == uuid, cancellationToken);
		if (pending is null)
			return FailedDependency(ErrorResponse.InvalidPendingId);

		var unpackedArchivePath = pending.UnpackedArchivePath;
		await using (var archiveStream = System.IO.File.OpenRead(pending.ArchivePath))
		{
			ArchiveHelper.UnpackInto(archiveStream, unpackedArchivePath);
		}

		try
		{
			PubSpec pubSpec;
			try
			{
				pubSpec = await GetPubSpec(unpackedArchivePath, cancellationToken);
			}
			catch (FileNotFoundException)
			{
				return UnprocessableEntity(ErrorResponse.MissingPubspec);
			}
			catch (Exception ex)
			{
				return UnprocessableEntity(ErrorResponse.InvalidPubspec(ex.Message));
			}

			var packageName = pubSpec.Name;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (packageName is null)
				return UnprocessableEntity(ErrorResponse.InvalidPubspec("The pubspec.yaml is missing a package name"));

			var packageVersionId = pubSpec.Version;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (packageVersionId is null)
				return UnprocessableEntity(
					ErrorResponse.InvalidPubspec("The pubspec.yaml is missing a package version"));

			if (!SemVersion.TryParse(packageVersionId, SemVersionStyles.Any, out var packageVersionSemver))
				return UnprocessableEntity(ErrorResponse.InvalidPubspec("The package version could not be parsed"));

			var package = await db.Packages.Where(p => p.Name == packageName)
				.Include(p => p.Versions)
				.FirstOrDefaultAsync(cancellationToken);

			if (package is null)
			{
				package = new()
				{
					Author = pending.Uploader,
					Name = packageName,
					Versions = new List<PackageVersion>(),
					IsDiscontinued = false,
					ReplacedBy = null,
					Latest = null,
				};

				db.Packages.Add(package);
				await db.SaveChangesAsync(cancellationToken);
			}
			else
			{
				if (package.Author != pending.Uploader)
					return Unauthorized(ErrorResponse.PackageAuthorMismatch);

				if (package.IsDiscontinued)
					return UnprocessableEntity(ErrorResponse.PackageDiscontinued);

				if (package.Versions.Any(v => v.Version == packageVersionId))
					return UnprocessableEntity(ErrorResponse.VersionAlreadyExists(packageName, packageVersionId));

				if (package.Latest is not null)
				{
					var latestSemver = SemVersion.Parse(package.Latest.Version, SemVersionStyles.Any);
					if (packageVersionSemver.ComparePrecedenceTo(latestSemver) != 1)
						return UnprocessableEntity(ErrorResponse.VersionOlderThanLatest(package.Latest.Version));
				}
			}

			var archiveEntry = FilesystemFileEntry.FromPath(pending.ArchivePath);
			var archiveSha256 =
				await storageProvider.StoreArchiveAsync(packageName, packageVersionId, archiveEntry, cancellationToken);

			var packageVersion = new PackageVersion
			{
				PubSpec = pubSpec,
				PackageName = packageName,
				Version = packageVersionId,
				ArchiveUrl = endpointHelper.GenerateFullyQualified(Request,
					$"/api/packages/{packageName}/versions/{packageVersionId}.tar.gz"),
				ArchiveSha256 = archiveSha256.Value,
				PublishedAtUtc = DateTimeOffset.UtcNow,
			};

			package.Versions.Add(packageVersion);
			package.Latest = packageVersion;
			db.PendingArchives.Remove(pending);

			await db.SaveChangesAsync(cancellationToken);

			System.IO.File.Delete(pending.ArchivePath);

			Response.Headers.ContentType = new[] { "application/vnd.pub.v2+json" };
			return Ok(new SuccessResponseDto(new($"Successfully uploaded {packageName} version {packageVersionId}! " +
				endpointHelper.GenerateFullyQualified(Request,
					$"/api/packages/{packageName}/versions/{packageVersionId}"))));
		}
		finally
		{
			// clean up unpacked archive
			if (Directory.Exists(unpackedArchivePath))
				Directory.Delete(unpackedArchivePath, true);
		}
	}

	private static async Task<PubSpec> GetPubSpec(string workingDirectory,
		CancellationToken cancellationToken = default)
	{
		var pubSpecPath = await PathHelper.GetCaseInsensitivePath(workingDirectory, "pubspec.yaml", cancellationToken);
		if (!System.IO.File.Exists(pubSpecPath))
			throw new FileNotFoundException("pubspec.yaml not found in working directory", pubSpecPath);

		var pubSpecText = await System.IO.File.ReadAllTextAsync(pubSpecPath, cancellationToken);
		var yamlDeser = new DeserializerBuilder()
			.WithNamingConvention(UnderscoredNamingConvention.Instance)
			.IgnoreUnmatchedProperties()
			.Build();

		return yamlDeser.Deserialize<PubSpec>(pubSpecText);
	}
}
