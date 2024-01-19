using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart")]
[Tags("Dart")]
public class DartRootController : DartController
{
	[Authorize]
	[HttpPost("Create")]
	[ProducesResponseType(StatusCodes.Status302Found)]
	public Task<IActionResult> CreateNewAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Search")]
	public Task<IActionResult> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
