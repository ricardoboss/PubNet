using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("packages")]
[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
public class PackagesController : BaseController
{
    private readonly ILogger<PackagesController> _logger;
    private readonly PubNetContext _db;
    private readonly IPackageStorageProvider _storageProvider;

    public PackagesController(ILogger<PackagesController> logger, PubNetContext db, IPackageStorageProvider storageProvider)
    {
        _logger = logger;
        _db = db;
        _storageProvider = storageProvider;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchPackagesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ResponseCache(VaryByQueryKeys = new []{ "q", "before", "limit" }, Location = ResponseCacheLocation.Any, Duration = 300)]
    public IActionResult Get([FromQuery] string? q = null, [FromQuery] long? before = null, [FromQuery] int? limit = null)
    {
        const int maxLimit = 1000;

        IQueryable<Package> packages = _db.Packages
            .Where(p => p.Latest != null)
            .OrderByDescending(p => p.Latest!.PublishedAtUtc)
        ;

        if (q != null) packages = packages.Where(p => p.Name.StartsWith(q));

        if (before.HasValue)
        {
            if (!limit.HasValue) return BadRequest(ErrorResponse.InvalidQuery);

            var publishedAtUpperLimit = DateTimeOffset.FromUnixTimeMilliseconds(before.Value);

            packages = packages.Where(p => p.Latest!.PublishedAtUtc < publishedAtUpperLimit);
        }

        if (limit.HasValue)
        {
            var resultLimit = Math.Min(limit.Value, maxLimit);

            packages = packages.Take(resultLimit);
        }

        return Ok(new SearchPackagesResponse(packages.ToList().Select(p => new SearchResultPackage(p.Name, p.ReplacedBy, p.IsDiscontinued, p.Author?.UserName, p.Latest!.Version, p.Latest!.PublishedAtUtc))));
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Package))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["PackageName"] = name,
               }))
        {
            var package = await _db.Packages
                .Where(p => p.Name == name)
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(cancellationToken);

            return package is null ? NotFound() : Ok(package);
        }
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> DeleteByName(string name, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["PackageName"] = name,
                   ["AuthorUsername"] = author.UserName,
               }))
        {
            var package = await _db.Packages
                .Where(p => p.Name == name)
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(cancellationToken);

            if (package is null) return NotFound();

            if (author.Id != package.AuthorId) return Unauthorized(ErrorResponse.PackageAuthorMismatch);

            // TODO: remove fiels from storage

            _db.PackageVersionAnalyses.RemoveRange(_db.PackageVersionAnalyses.Include(a => a.Version).Where(a => package.Versions.Any(v => v == a.Version)));
            _db.PackageVersions.RemoveRange(package.Versions);
            _db.Packages.Remove(package);

            await _db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }

    [HttpGet("{name}/versions/{version}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageVersion))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVersion(string name, string version, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["PackageName"] = name,
               }))
        {
            var package = await _db.Packages
                .Where(p => p.Name == name)
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(cancellationToken);

            var packageVersion = package?.Versions.FirstOrDefault(v => v.Version == version);

            if (packageVersion is null)
                return NotFound();

            Response.Headers.ContentType = "application/vnd.pub.v2+json";
            return Ok(packageVersion);
        }
    }

    [HttpGet("{name}/versions/{version}.tar.gz")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetVersionArchive(string name, string version)
    {
        try
        {
            var stream = _storageProvider.ReadArchive(name, version);

            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = $"{name}-{version}.tar.gz",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading archive for package \"{Package}\" version \"{Version}\"", name, version);

            return NotFound();
        }
    }

    [HttpGet("versions/new")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadEndpointData))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> VersionsNew([FromServices] IUploadEndpointGenerator uploadEndpointGenerator, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
    {
        var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["AuthorUsername"] = author.UserName,
               }))
        {
            var data = await uploadEndpointGenerator.GenerateUploadEndpointData(Request, author, cancellationToken);

            return Ok(data);
        }
    }
}
