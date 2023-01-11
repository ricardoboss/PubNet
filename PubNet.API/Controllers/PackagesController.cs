using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackagesResponse))]
    [OutputCache(VaryByQueryKeys = new []{ "q" })]
    public IActionResult Get([FromQuery] string? q = null)
    {
        IEnumerable<Package> packages = _db.Packages;

        if (q != null)
        {
            packages = packages.Where(p => p.Name.StartsWith(q));
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