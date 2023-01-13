using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.API.Models;
using PubNet.API.Services;

namespace PubNet.API.Controllers;

[ApiController]
[Route("packages")]
public class PackagesController : ControllerBase
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackagesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ResponseCache(VaryByQueryKeys = new []{ "q", "before", "limit" }, Location = ResponseCacheLocation.Any, Duration = 300)]
    public IActionResult Get([FromQuery] string? q = null, [FromQuery] long? before = null, [FromQuery] int? limit = null)
    {
        const int maxLimit = 1000;

        IQueryable<Package> packages = _db.Packages
            .Where(p => p.Latest != null)
            .Include(p => p.Latest)
            .OrderByDescending(p => p.Latest!.PublishedAtUtc)
        ;

        if (q != null)
        {
            packages = packages.Where(p => p.Name.StartsWith(q));
        }

        if (before.HasValue)
        {
            if (!limit.HasValue)
            {
                return BadRequest(new ErrorResponse(new("invalid-query", "Query parameter 'limit' is mandatory if 'before' is given")));
            }

            var publishedAtUpperLimit = DateTimeOffset.FromUnixTimeMilliseconds(before.Value);

            packages = packages.Where(p => p.Latest!.PublishedAtUtc < publishedAtUpperLimit);
        }

        if (limit.HasValue)
        {
            var resultLimit = Math.Min(limit.Value, maxLimit);

            packages = packages.Take(resultLimit);
        }

        return Ok(new PackagesResponse(packages));
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Package))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["PackageName"] = name,
               }))
        {
            var package = await _db.Packages.Where(p => p.Name == name).Include(p => p.Versions).FirstOrDefaultAsync();

            return package is null ? NotFound() : Ok(package);
        }
    }

    [HttpGet("{name}/versions/{version}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageVersion))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVersion(string name, string version)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["PackageName"] = name,
               }))
        {
            var package = await _db.Packages.Where(p => p.Name == name).Include(p => p.Versions).FirstOrDefaultAsync();
            var packageVersion = package?.Versions.FirstOrDefault(v => v.Version == version);

            return packageVersion is null ? NotFound() : Ok(packageVersion);
        }
    }

    [HttpGet("{name}/versions/{version}.tar.gz")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetVersionArchive(string name, string version)
    {
        try
        {
            return _storageProvider.ReadArchive(name, version);
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
    public async Task<IActionResult> VersionsNew([FromServices] IUploadEndpointGenerator uploadEndpointGenerator, [FromServices] ApplicationRequestContext context)
    {
        var authorToken = context.RequireAuthorToken();
        var response = await uploadEndpointGenerator.GenerateUploadEndpointData(Request, authorToken);
        return Ok(response);
    }
}