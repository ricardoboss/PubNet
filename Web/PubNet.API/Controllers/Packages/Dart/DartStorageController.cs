using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/Storage/{pendingId}")]
[Tags("Dart")]
public class DartStorageController : DartController
{
	[HttpPut]
	public Task<IActionResult> UploadAsync(string pendingId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	public Task<IActionResult> FinalizeAsync(string pendingId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
