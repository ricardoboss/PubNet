using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Dart;

[ApiController]
[Route("packages/dart/storage")]
public class DartStorageController : ControllerBase
{
	[HttpPut("{pendingId}")]
	public Task<IActionResult> UploadAsync(string pendingId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("{pendingId}")]
	public Task<IActionResult> FinalizeAsync(string pendingId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
