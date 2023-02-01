using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Common.Interfaces;
using PubNet.Common.Utils;
using PubNet.Database;
using PubNet.Database.Models;
using Semver;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PubNet.API.Controllers;

[ApiController]
[Route("storage")]
public class StorageController : BaseController, IUploadEndpointGenerator
{
    private readonly ILogger<StorageController> _logger;
    private readonly PubNetContext _db;
    private readonly IPackageStorageProvider _storageProvider;
    private readonly EndpointHelper _endpointHelper;

    public StorageController(ILogger<StorageController> logger, PubNetContext db, IPackageStorageProvider storageProvider, EndpointHelper endpointHelper)
    {
        _logger = logger;
        _db = db;
        _storageProvider = storageProvider;
        _endpointHelper = endpointHelper;
    }

    /// <inheritdoc />
    [NonAction]
    public Task<UploadEndpointData> GenerateUploadEndpointData(HttpRequest request, Author author, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var url = _endpointHelper.GenerateFullyQualified(request, "/api/storage/upload");
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
                return StatusCode(StatusCodes.Status413PayloadTooLarge, ErrorResponse.PackagePayloadTooLarge(maxUploadSize));
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

        var pending = new PendingArchive
        {
            Uuid = pendingId,
            ArchivePath = tempFile,
            Uploader = author,
            UploadedAtUtc = DateTimeOffset.UtcNow,
        };

        _db.PendingArchives.Add(pending);
        await _db.SaveChangesAsync();

        _logger.LogTrace("Added pending archive {ArchiveId}", pending.Uuid);

        var finalizeUrl = _endpointHelper.GenerateFullyQualified(
            Request,
            "/api/storage/finalize",
            new Dictionary<string, string?>
            {
                { "pendingId", pendingId.ToString() },
            },
            true
        );

        _logger.LogTrace("Generated finalize url for archive {ArchiveId} to {FinalizeUrl}", pending.Uuid, finalizeUrl);

        Response.Headers.Location = finalizeUrl;
        return NoContent();
    }

    [HttpGet("finalize")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> FinalizeUpload([FromQuery] string? pendingId, CancellationToken cancellationToken = default)
    {
        if (!_endpointHelper.ValidateSignature(Request.QueryString.ToString()))
            return BadRequest(ErrorResponse.InvalidSignedUrl);

        if (pendingId is null)
            return FailedDependency(ErrorResponse.MissingPendingId);

        if (!Guid.TryParse(pendingId, out var uuid))
            return FailedDependency(ErrorResponse.InvalidPendingId);

        var pending = await _db.PendingArchives
            .Include(p => p.Uploader)
            .FirstOrDefaultAsync(a => a.Uuid == uuid, cancellationToken);
        if (pending is null)
            return FailedDependency(ErrorResponse.InvalidPendingId);

        var unpackedArchivePath = pending.UnpackedArchivePath;
        await using (var archiveStream = System.IO.File.OpenRead(pending.ArchivePath))
            ArchiveHelper.UnpackInto(archiveStream, unpackedArchivePath);

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
            if (packageName is null)
                return UnprocessableEntity(ErrorResponse.InvalidPubspec("The pubspec.yaml is missing a package name"));

            var packageVersionId = pubSpec.Version;
            if (packageVersionId is null)
                return UnprocessableEntity(ErrorResponse.InvalidPubspec("The pubspec.yaml is missing a package version"));

            if (!SemVersion.TryParse(packageVersionId, SemVersionStyles.Any, out var packageVersionSemver))
                return UnprocessableEntity(ErrorResponse.InvalidPubspec("The package version could not be parsed"));

            var package = await _db.Packages.Where(p => p.Name == packageName)
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

                _db.Packages.Add(package);
                await _db.SaveChangesAsync(cancellationToken);
            }
            else
            {
                if (package.Author != pending.Uploader)
                    return Unauthorized(ErrorResponse.PackageAuthorMismatch);

                if (package.Versions.Any(v => v.Version == packageVersionId))
                    return UnprocessableEntity(ErrorResponse.VersionAlreadyExists(packageName, packageVersionId));

                if (package.Latest is not null)
                {
                    var latestSemver = SemVersion.Parse(package.Latest.Version, SemVersionStyles.Any);
                    if (packageVersionSemver.ComparePrecedenceTo(latestSemver) != 1)
                        return UnprocessableEntity(ErrorResponse.VersionOlderThanLatest(package.Latest.Version));
                }
            }

            string archiveSha256;
            await using (var archiveStream = System.IO.File.OpenRead(pending.ArchivePath))
                archiveSha256 = await _storageProvider.StoreArchive(packageName, packageVersionId, archiveStream, cancellationToken);

            var packageVersion = new PackageVersion
            {
                PubSpec = pubSpec,
                PackageName = packageName,
                Version = packageVersionId,
                ArchiveUrl = _endpointHelper.GenerateFullyQualified(Request, $"/api/packages/{packageName}/versions/{packageVersionId}.tar.gz"),
                ArchiveSha256 = archiveSha256,
                PublishedAtUtc = DateTimeOffset.UtcNow,
            };

            package.Versions.Add(packageVersion);
            package.Latest = packageVersion;
            _db.PendingArchives.Remove(pending);

            await _db.SaveChangesAsync(cancellationToken);

            System.IO.File.Delete(pending.ArchivePath);

            Response.Headers.ContentType = new[] { "application/vnd.pub.v2+json" };
            return Ok(new SuccessResponse(new($"Successfully uploaded {packageName} version {packageVersionId}! " + _endpointHelper.GenerateFullyQualified(Request, $"/api/packages/{packageName}/versions/{packageVersionId}"))));
        }
        finally
        {
            // clean up unpacked archive
            if (Directory.Exists(unpackedArchivePath))
                Directory.Delete(unpackedArchivePath, true);
        }
    }

    private static async Task<PubSpec> GetPubSpec(string workingDirectory, CancellationToken cancellationToken = default)
    {
        var pubSpecPath = Path.Combine(workingDirectory, "pubspec.yaml");
        if (!System.IO.File.Exists(pubSpecPath))
            throw new FileNotFoundException("pubspec.yaml not found in working directory", pubSpecPath);

        var pubSpecText = await System.IO.File.ReadAllTextAsync(pubSpecPath, cancellationToken);
        var yamlDeser = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        return yamlDeser.Deserialize<PubSpec>(pubSpecText);
    }
}