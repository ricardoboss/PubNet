using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages;

[ApiController]
[Route("Packages")]
[Tags("Packages")]
public class PackagesRootController : PackagesController
{
	[HttpGet("Search")]
	public Task<IActionResult> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
