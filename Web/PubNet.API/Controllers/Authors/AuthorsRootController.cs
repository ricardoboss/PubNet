using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Authors;

[Route("Authors")]
[Tags("Authors")]
public class AuthorsRootController : AuthorsController
{
	[HttpGet("Search")]
	public Task<IActionResult> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken? cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
