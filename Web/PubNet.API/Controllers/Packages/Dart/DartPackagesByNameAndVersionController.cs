using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Abstractions.Packages.Dart.Docs;
using PubNet.API.Attributes;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Web;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/{name}/{version}")]
[Tags("Dart")]
public class DartPackagesByNameAndVersionController(
	IDartPackageVersionAnalysisProvider analysisProvider,
	IDartPackageVersionDocsProviderFactory docsProviderFactory,
	IMimeTypeProvider mimeTypeProvider,
	IDartPackageArchiveProvider archiveProvider,
	IDartPackageDmo dartPackageDmo
) : DartController
{
	[HttpGet("analysis.json")]
	[ProducesResponseType<DartPackageVersionAnalysisDto>(StatusCodes.Status200OK)]
	public async Task<DartPackageVersionAnalysisDto?> GetAnalysisAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		var analysis = await analysisProvider.GetAnalysisAsync(name, version, cancellationToken);
		return analysis is null
			? null
			: DartPackageVersionAnalysisDto.MapFrom(analysis);
	}

	[HttpGet("archive.tar.gz")]
	[ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetArchiveAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		var stream = await archiveProvider.GetArchiveAsync(name, version, cancellationToken);
		if (stream is null)
			return NotFound();

		return File(stream, "application/gzip", $"{name}-{version}.tar.gz");
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

	[HttpPatch("Retract")]
	[Authorize, RequireScope(Scopes.Dart.Retract)]
	[ProducesResponseType<DartSuccessDto>(StatusCodes.Status200OK)]
	public async Task<DartSuccessDto> RetractAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		await dartPackageDmo.RetractAsync(name, version, cancellationToken);

		return DartSuccessDto.WithMessage($"Package '{name}' version '{version}' has been retracted");
	}
}
