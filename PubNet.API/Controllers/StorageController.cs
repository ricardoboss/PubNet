using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.API.Models;
using PubNet.API.Services;
using PubNet.API.Utils;
using PubNet.API.WorkerTasks;
using PubNet.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PubNet.API.Controllers;

[ApiController]
[Route("storage")]
public class StorageController : ControllerBase, IUploadEndpointGenerator
{
    private readonly ILogger<StorageController> _logger;
    private readonly PubNetContext _db;
    private readonly IPackageStorageProvider _storageProvider;
    private readonly WorkerTaskQueue _workerTaskQueue;
    private readonly EndpointHelper _endpointHelper;

    public StorageController(ILogger<StorageController> logger, PubNetContext db, IPackageStorageProvider storageProvider, WorkerTaskQueue workerTaskQueue, EndpointHelper endpointHelper)
    {
        _logger = logger;
        _db = db;
        _storageProvider = storageProvider;
        _workerTaskQueue = workerTaskQueue;
        _endpointHelper = endpointHelper;
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
                return StatusCode(StatusCodes.Status411LengthRequired, new ErrorResponse(new("length-required", "The Content-Length header is required")));
            case > maxUploadSize:
                return StatusCode(StatusCodes.Status413PayloadTooLarge, new ErrorResponse(new("payload-too-large", $"Maximum payload size is {maxUploadSize} bytes")));
        }

        var packageFile = Request.Form.Files.FirstOrDefault(f => f.Name == "file");
        if (packageFile is null)
            return BadRequest(new ErrorResponse(new("missing-package-file", "The package file is missing")));

        if (!Request.Form.ContainsKey("token-id"))
            return BadRequest(new ErrorResponse(new("missing-fields", "Not all fields have been forwarded")));

        var authorTokenId = int.Parse(Request.Form["token-id"].ToString());
        var authorToken = await _db.Tokens.FindAsync(authorTokenId);
        if (authorToken is null)
            return BadRequest(new ErrorResponse(new("invalid-token", "Invalid token id provided")));

        var pendingId = Guid.NewGuid();
        var tempFile = Path.GetTempPath() + pendingId + ".tar.gz";

        _logger.LogDebug("Storing package archive for {Author} at {Path}", authorToken.Owner.UserName, tempFile);

        await using (var fileStream = System.IO.File.OpenWrite(tempFile)) {
            await packageFile.CopyToAsync(fileStream);
        }

        var pending = new PendingArchive
        {
            Uuid = pendingId,
            ArchivePath = tempFile,
            Uploader = authorToken,
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
            return BadRequest(new ErrorResponse(new("invalid-url", "The provided signature is invalid indicating the url has been tempered with")));

        if (pendingId is null)
            return StatusCode(StatusCodes.Status424FailedDependency, new ErrorResponse(new("missing-id", "No pending id was provided")));

        if (!Guid.TryParse(pendingId, out var uuid))
            return BadRequest(new ErrorResponse(new("invalid-id", "An invalid pending id was provided")));

        var pending = await _db.PendingArchives.FirstOrDefaultAsync(a => a.Uuid == uuid, cancellationToken);
        if (pending is null)
            return StatusCode(StatusCodes.Status424FailedDependency, new ErrorResponse(new("invalid-id", "An invalid pending id was provided")));

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
                return UnprocessableEntity(new ErrorResponse(new("missing-pubspec", "The package archive is missing a pubspec.yaml")));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(new("invalid-pubspec",
                    $"An error occurred while parsing the pubspec.yaml: {ex.Message}")));
            }

            var packageName = pubSpec.Name;
            if (packageName is null)
                return UnprocessableEntity(new ErrorResponse(new("invalid-pubspec", "The pubspec.yaml is missing a package name")));

            var packageVersionId = pubSpec.Version;
            if (packageVersionId is null)
                return UnprocessableEntity(new ErrorResponse(new("invalid-pubspec", "The pubspec.yaml is missing a package version")));

            var package = await _db.Packages.Where(p => p.Name == packageName)
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(cancellationToken);

            if (package is null)
            {
                package = new()
                {
                    Author = pending.Uploader.Owner,
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
                if (package.Versions.Any(v => v.Version == packageVersionId))
                    return BadRequest(new ErrorResponse(new("version-already-exists", $"Version {packageVersionId} of {packageName} already exists")));

                // TODO: check only newer versions get uploaded
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

            _workerTaskQueue.Enqueue(new PubSpecAnalyzerTask(packageName, packageVersionId));

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

    /// <inheritdoc />
    [NonAction]
    public Task<UploadEndpointData> GenerateUploadEndpointData(HttpRequest request, AuthorToken authorToken, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var url = _endpointHelper.GenerateFullyQualified(request, "/api/storage/upload");
        var fields = new Dictionary<string, string>
        {
            { "token-id", authorToken.Id.ToString() },
        };

        return Task.FromResult(new UploadEndpointData(url, fields));
    }
}