using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Abstractions.Packages.Dart.Docs;
using PubNet.API.Attributes;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Auth;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/{name}/Versions/{version}")]
[Tags("Dart")]
public class DartPackagesByNameAndVersionController(
	IDartPackageVersionAnalysisProvider analysisProvider,
	IDartPackageVersionDocsProviderFactory docsProviderFactory,
	IMimeTypeProvider mimeTypeProvider,
	IDartPackageArchiveProvider archiveProvider,
	IDartPackageDao dartPackageDao,
	IDartPackageDmo dartPackageDmo
) : DartController
{
	[HttpGet]
	[ProducesResponseType<DartPackageVersionDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		var package = await dartPackageDao.GetByNameAsync(name, cancellationToken);
		if (package is null)
			return NotFoundDto("package-not-found", $"Package '{name}' not found");

		var matchingVersion = string.Equals(version, "latest", StringComparison.OrdinalIgnoreCase)
			? package.LatestVersion
			: package.Versions.FirstOrDefault(v => v.Version == version);

		if (matchingVersion is null)
			return NotFoundDto("package-version-not-found", $"Package '{name}' version '{version}' not found");

		var (uri, hash) = archiveProvider.GetArchiveUriAndHash(name, version);

		return Ok(DartPackageVersionDto.MapFrom(matchingVersion, uri, hash));
	}

	[HttpGet("analysis.json")]
	[ProducesResponseType<DartPackageVersionAnalysisDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAnalysisAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		var analysis = await analysisProvider.GetAnalysisAsync(name, version, cancellationToken);
		return analysis is null
			? NotFoundDto("analysis-not-found", "Package not found or analysis not completed yet")
			: Ok(DartPackageVersionAnalysisDto.MapFrom(analysis));
	}

	[HttpGet("archive.tar.gz")]
	[ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetArchiveAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		var stream = await archiveProvider.GetArchiveContentAsync(name, version, cancellationToken);
		if (stream is null)
			return NotFoundDto("archive-not-found", $"Package '{name}' version '{version}' or archive not found");

		return File(stream, "application/gzip", $"{name}-{version}.tar.gz");
	}

	[HttpGet("Docs/{**path}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetDocsAsync(string name, string version, string path,
		CancellationToken cancellationToken = default)
	{
		var docsProvider = await docsProviderFactory.CreateAsync(name, version, cancellationToken);

		var file = docsProvider.GetFileInfo(path);
		if (!file.Exists)
		{
			Response.StatusCode = StatusCodes.Status404NotFound;

			file = docsProvider.GetNotFoundFile();
		}

		var mimeType = mimeTypeProvider.GetMimeType(file.Name);

		var readStream = file.CreateReadStream();

		Response.RegisterForDisposeAsync(readStream);

		return File(readStream, mimeType, file.Name);
	}

	[HttpPatch("Retract")]
	[Authorize, RequireScope(Scopes.Packages.Dart.Retract)]
	[ProducesResponseType<DartSuccessDto>(StatusCodes.Status200OK)]
	public async Task<DartSuccessDto> RetractAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		await dartPackageDmo.RetractAsync(name, version, cancellationToken);

		return DartSuccessDto.WithMessage("package_version_retracted",
			$"Package '{name}' version '{version}' has been retracted");
	}
}
