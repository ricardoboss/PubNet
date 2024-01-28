using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Packages.Dart.Spec;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart")]
[Tags("Dart")]
public class DartRootController : DartController
{
	[Authorize]
	[HttpPost("Versions/New")]
	public Task<DartNewVersionDto> CreateNewAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Search")]
	public Task<IActionResult> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
