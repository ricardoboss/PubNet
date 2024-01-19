using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers;

[Route("[controller]/[action]")]
[Tags("Authentication")]
public class AuthenticationController : ControllerBase
{
	[HttpPost]
	public Task<IActionResult> Login(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpPost]
	public Task<IActionResult> Register(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
