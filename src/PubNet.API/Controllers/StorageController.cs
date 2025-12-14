using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.DTO.Authentication.Errors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Errors;
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
	public Task<UploadEndpointDataDto> GenerateUploadEndpointData(HttpRequest request, Author author,
		CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var url = endpointHelper.GenerateFullyQualified(request, "/api/storage/upload", new Dictionary<string, string?>
		{
			{ "authorId", author.Id.ToString() },
		});

		url = endpointHelper.SignEndpoint(url);

		return Task.FromResult(new UploadEndpointDataDto(url, new()));
	}

	[HttpPost("upload")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(PubNetStatusCodes.Status411LengthRequired, Type = typeof(LengthRequiredErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status413PayloadTooLarge, Type = typeof(PayloadTooLargeErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status470MissingRequiredData, Type = typeof(MissingRequiredDataErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status471InvalidUploadData, Type = typeof(InvalidUploadDataErrorDto))]
	public async Task<IActionResult> Upload([FromQuery] int? authorId)
	{
		if (!endpointHelper.ValidateSignature(Request.QueryString.ToString()))
			return Error<InvalidUploadDataErrorDto>(PubNetStatusCodes.Status471InvalidUploadData,
				"Invalid query signature");

		const long maxUploadSize = 100_000_000; // 100 MB

		var size = Request.Headers.ContentLength;
		switch (size)
		{
			case null:
				return Error<LengthRequiredErrorDto>(PubNetStatusCodes.Status411LengthRequired);
			case > maxUploadSize:
				return Error<PayloadTooLargeErrorDto>(PubNetStatusCodes.Status413PayloadTooLarge,
					$"Maximum payload size is {maxUploadSize} bytes");
		}

		var packageFile = Request.Form.Files.FirstOrDefault(f => f.Name == "file");
		if (packageFile is null)
			return Error<MissingRequiredDataErrorDto>(PubNetStatusCodes.Status470MissingRequiredData,
				"Missing package file");

		if (authorId is null)
			return Error<MissingRequiredDataErrorDto>(PubNetStatusCodes.Status470MissingRequiredData,
				"Missing author id");

		var author = await db.Authors.FindAsync(authorId);
		if (author is null)
			return Error<InvalidUploadDataErrorDto>(PubNetStatusCodes.Status471InvalidUploadData,
				"Author id not found: " + authorId);

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
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessMessageDto))]
	[ProducesResponseType(PubNetStatusCodes.Status403Forbidden, Type = typeof(ForbiddenErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status470MissingRequiredData, Type = typeof(MissingRequiredDataErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status471InvalidUploadData, Type = typeof(InvalidUploadDataErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status472InvalidPubSpec, Type = typeof(InvalidPubSpecErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status473PackageDiscontinued, Type = typeof(PackageDiscontinuedErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status474VersionConflict, Type = typeof(VersionConflictErrorDto))]
	public async Task<IActionResult> FinalizeUpload([FromQuery] string? pendingId,
		CancellationToken cancellationToken = default)
	{
		if (!endpointHelper.ValidateSignature(Request.QueryString.ToString()))
			return Error<InvalidUploadDataErrorDto>(PubNetStatusCodes.Status471InvalidUploadData,
				"Invalid query signature");

		if (pendingId is null)
			return Error<MissingRequiredDataErrorDto>(PubNetStatusCodes.Status470MissingRequiredData,
				"Missing pending id");

		if (!Guid.TryParse(pendingId, out var uuid))
			return Error<InvalidUploadDataErrorDto>(PubNetStatusCodes.Status471InvalidUploadData, "Invalid pending id");

		var pending = await db.PendingArchives
			.Include(p => p.Uploader)
			.FirstOrDefaultAsync(a => a.Uuid == uuid, cancellationToken);
		if (pending is null)
			return Error<InvalidUploadDataErrorDto>(PubNetStatusCodes.Status471InvalidUploadData,
				"Pending id not found: " + pendingId);

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
				return Error<MissingRequiredDataErrorDto>(PubNetStatusCodes.Status470MissingRequiredData,
					"Missing pubspec.yaml");
			}
			catch (Exception ex)
			{
				return Error<InvalidPubSpecErrorDto>(PubNetStatusCodes.Status472InvalidPubSpec,
					$"An error occurred while parsing the pubspec.yaml: {ex.Message}");
			}

			var packageName = pubSpec.Name;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (packageName is null)
				return Error<InvalidPubSpecErrorDto>(PubNetStatusCodes.Status472InvalidPubSpec,
					"The pubspec.yaml is missing a package name");

			var packageVersionId = pubSpec.Version;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (packageVersionId is null)
				return Error<InvalidPubSpecErrorDto>(PubNetStatusCodes.Status472InvalidPubSpec,
					"The pubspec.yaml is missing a package version");

			if (!SemVersion.TryParse(packageVersionId, SemVersionStyles.Any, out var packageVersionSemver))
				return Error<InvalidPubSpecErrorDto>(PubNetStatusCodes.Status472InvalidPubSpec,
					"The package version could not be parsed");

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
					return Error<ForbiddenErrorDto>(PubNetStatusCodes.Status403Forbidden, "You don't own this package");

				if (package.IsDiscontinued)
					return Error<PackageDiscontinuedErrorDto>(PubNetStatusCodes.Status473PackageDiscontinued);

				if (package.Versions.Any(v => v.Version == packageVersionId))
					return Error<VersionConflictErrorDto>(PubNetStatusCodes.Status474VersionConflict,
						$"Version {packageVersionId} of {packageName} already exists");

				if (package.Latest is not null)
				{
					var latestSemver = SemVersion.Parse(package.Latest.Version, SemVersionStyles.Any);
					if (packageVersionSemver.ComparePrecedenceTo(latestSemver) != 1)
						return Error<VersionConflictErrorDto>(PubNetStatusCodes.Status474VersionConflict,
							$"The version you are trying to upload is older than the latest version ({package.Latest.Version})");
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

			var linkToPackage =
				endpointHelper.GenerateFullyQualified(Request,
					$"/api/packages/{packageName}/versions/{packageVersionId}");

			Response.Headers.ContentType = new[] { "application/vnd.pub.v2+json" };
			return Ok(new SuccessMessageDto
			{
				Success = new()
				{
					Message = $"Successfully uploaded {packageName} version {packageVersionId}! " + linkToPackage,
					Code = "upload-successful",
				},
			});
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
