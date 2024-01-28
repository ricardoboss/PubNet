using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Abstractions.Packages.Dart.Docs;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/{name}/{version}")]
[Tags("Dart")]
public class DartPackagesByNameAndVersionController(
	IDartPackageVersionAnalysisProvider analysisProvider,
	IDartPackageVersionDocsProviderFactory docsProviderFactory,
	IMimeTypeProvider mimeTypeProvider
) : DartController
{
	[HttpGet("analysis.json")]
	[ProducesResponseType(typeof(DartPackageVersionAnalysisDto), StatusCodes.Status200OK)]
	public async Task<DartPackageVersionAnalysisDto?> GetAnalysisAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		var analysis = await analysisProvider.GetAnalysisAsync(name, version, cancellationToken);
		return analysis is null
			? null
			: DartPackageVersionAnalysisDto.MapFrom(analysis);
	}

	[HttpGet("archive.tar.gz")]
	[ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
	public Task<IActionResult> GetArchiveAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Docs/{**path}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetDocsAsync(string name, string version, string path, CancellationToken cancellationToken = default)
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

	[Authorize]
	[HttpPatch("Retract")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<DartSuccessDto> RetractAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
