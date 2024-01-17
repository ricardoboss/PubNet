using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.Extensions;
using PubNet.API.Interfaces;
using PubNet.Common.Utils;
using PubNet.Database.Context;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Dart;
using PubNet.PackageStorage.Abstractions;
using Semver;
using SignedUrl.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PubNet.API.Controllers;

[ApiController]
[Route("storage")]
public class StorageController : BaseController, IUploadEndpointGenerator
{
	private readonly PubNetContext _db;
	private readonly IUrlSigner _urlSigner;
	private readonly ILogger<StorageController> _logger;
	private readonly IArchiveStorage _archiveStorage;

	public StorageController(ILogger<StorageController> logger, PubNetContext db,
		IArchiveStorage archiveStorage, IUrlSigner urlSigner)
	{
		_logger = logger;
		_db = db;
		_archiveStorage = archiveStorage;
		_urlSigner = urlSigner;
	}

	/// <inheritdoc />
	[NonAction]
	public Task<UploadEndpointData> GenerateUploadEndpointData(HttpRequest request, Author author,
		CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var url = _urlSigner.GenerateFullyQualified(request, "/api/storage/upload");
		var fields = new Dictionary<string, string>
		{
			{ "author-id", author.Id.ToString() },
		};

		return Task.FromResult(new UploadEndpointData(url, fields));
	}

	[HttpPost("upload")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status411LengthRequired, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status413PayloadTooLarge, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> Upload()
	{
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

		if (!Request.Form.ContainsKey("author-id"))
			return BadRequest(ErrorResponse.MissingFields);

		var authorId = int.Parse(Request.Form["author-id"].ToString());
		var author = await _db.Authors.FindAsync(authorId);
		if (author is null)
			return BadRequest(ErrorResponse.InvalidAuthorId);

		var pendingId = Guid.NewGuid();
		var tempFile = Path.GetTempPath() + pendingId + ".tar.gz";

		_logger.LogDebug("Storing package archive for {Author} at {Path}", author.UserName, tempFile);

		await using (var fileStream = System.IO.File.OpenWrite(tempFile))
		{
			await packageFile.CopyToAsync(fileStream);
		}

		var pending = new DartPendingArchive
		{
			Id = pendingId,
			ArchivePath = tempFile,
			Uploader = author,
			UploadedAt = DateTimeOffset.UtcNow,
		};

		_db.DartPendingArchives.Add(pending);
		await _db.SaveChangesAsync();

		_logger.LogTrace("Added pending archive {ArchiveId}", pending.Id);

		var finalizeUrl = _urlSigner.GenerateFullyQualified(
			Request,
			$"/api/storage/finalize?pendingId={pendingId}"
		);

		_logger.LogTrace("Generated finalize url for archive {ArchiveId} to {FinalizeUrl}", pending.Id, finalizeUrl);

		Response.Headers.Location = finalizeUrl;
		return NoContent();
	}

	[HttpGet("finalize")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
	[ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> FinalizeUpload([FromQuery] string? pendingId,
		CancellationToken cancellationToken = default)
	{
		if (!_urlSigner.Validate($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}"))
			return BadRequest(ErrorResponse.InvalidSignedUrl);

		if (pendingId is null)
			return FailedDependency(ErrorResponse.MissingPendingId);

		if (!Guid.TryParse(pendingId, out var uuid))
			return FailedDependency(ErrorResponse.InvalidPendingId);

		var pending = await _db.DartPendingArchives
			.Include(p => p.Uploader)
			.FirstOrDefaultAsync(a => a.Id == uuid, cancellationToken);
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

			var package = await _db.DartPackages.Where(p => p.Name == packageName)
				.Include(p => p.Versions)
				.FirstOrDefaultAsync(cancellationToken);

			if (package is null)
			{
				package = new()
				{
					Author = pending.Uploader,
					Name = packageName,
					Versions = new List<DartPackageVersion>(),
					IsDiscontinued = false,
					ReplacedBy = null,
					LatestVersion = null,
				};

				_db.DartPackages.Add(package);
				await _db.SaveChangesAsync(cancellationToken);
			}
			else
			{
				if (package.Author != pending.Uploader)
					return Unauthorized(ErrorResponse.PackageAuthorMismatch);

				if (package.IsDiscontinued)
					return UnprocessableEntity(ErrorResponse.PackageDiscontinued);

				if (package.Versions.Any(v => v.Version == packageVersionId))
					return UnprocessableEntity(ErrorResponse.VersionAlreadyExists(packageName, packageVersionId));

				if (package.LatestVersion is not null)
				{
					var latestSemver = SemVersion.Parse(package.LatestVersion.Version, SemVersionStyles.Any);
					if (packageVersionSemver.ComparePrecedenceTo(latestSemver) != 1)
						return UnprocessableEntity(ErrorResponse.VersionOlderThanLatest(package.LatestVersion.Version));
				}
			}

			string archiveSha256;
			await using (var archiveStream = System.IO.File.OpenRead(pending.ArchivePath))
			{
				archiveSha256 =
					await _archiveStorage.StoreArchiveAsync(pending.Uploader.UserName, packageName, packageVersionId, archiveStream,
						cancellationToken);
			}

			var packageVersion = new DartPackageVersion
			{
				PubSpec = pubSpec,
				Package = package,
				Version = packageVersionId,
				// ArchiveUrl = _urlSigner.GenerateFullyQualified(Request,
				// 	$"/api/packages/{packageName}/versions/{packageVersionId}.tar.gz"),
				// ArchiveSha256 = archiveSha256,
				PublishedAt = DateTimeOffset.UtcNow,
			};

			package.Versions.Add(packageVersion);
			package.LatestVersion = packageVersion;
			_db.DartPendingArchives.Remove(pending);

			await _db.SaveChangesAsync(cancellationToken);

			System.IO.File.Delete(pending.ArchivePath);

			Response.Headers.ContentType = new[] { "application/vnd.pub.v2+json" };
			return Ok(new SuccessResponse(new($"Successfully uploaded {packageName} version {packageVersionId}! " +
				_urlSigner.GenerateFullyQualified(Request,
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
			.Build();

		return yamlDeser.Deserialize<PubSpec>(pubSpecText);
	}
}
