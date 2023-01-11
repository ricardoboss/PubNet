using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("packages")]
public class PackagesController : ControllerBase
{
    private readonly ILogger<PackagesController> _logger;
    private readonly PubNetContext _db;

    public PackagesController(ILogger<PackagesController> logger, PubNetContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet("{name}")]
    public async Task<Package?> Get(string name)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["PackageName"] = name
               }))
        {
            return await _db.Packages.FirstOrDefaultAsync(p => p.Name == name);
        }
    }

    [HttpGet("versions/new")]
    public async Task VersionsNew()
    {
        throw new ArgumentException("test");
    }
}