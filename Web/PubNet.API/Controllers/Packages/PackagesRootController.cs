using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Packages;

namespace PubNet.API.Controllers.Packages;

[ApiController]
[Route("Packages")]
[Tags("Packages")]
public class PackagesRootController : PackagesController
{
	[HttpGet("Search")]
	public Task<PackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
