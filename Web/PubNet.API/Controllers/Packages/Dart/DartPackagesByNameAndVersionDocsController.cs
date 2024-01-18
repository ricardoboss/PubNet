using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Packages.Dart.Docs;

namespace PubNet.API.Controllers.Packages.Dart;

[ApiController]
[Route("packages/dart/{name}/{version}/docs/{**path}")]
public class DartPackagesByNameAndVersionDocsController(IDartPackageVersionDocsProviderFactory docsProviderFactory, IMimeTypeProvider mimeTypeProvider) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetFileAsync(string name, string version, string path, CancellationToken cancellationToken = default)
	{
		var docsProvider = await docsProviderFactory.CreateAsync(name, version, cancellationToken);

		var file = docsProvider.GetFileInfo(path);
		if (!file.Exists)
		{
			Response.StatusCode = StatusCodes.Status404NotFound;

			file = docsProvider.GetNotFoundFile();
		}

		var mimeType = mimeTypeProvider.GetMimeType(file.Name);

		return File(file.CreateReadStream(), mimeType, file.Name);
	}
}
