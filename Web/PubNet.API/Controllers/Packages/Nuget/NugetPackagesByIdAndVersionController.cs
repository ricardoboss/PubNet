﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/{id}/{version}")]
[Tags("Nuget")]
public class NugetPackagesByIdAndVersionController : NugetController
{
	[HttpGet("analysis.json")]
	public Task<IActionResult> GetAnalysisAsync(string id, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("archive.nupkg")]
	public Task<IActionResult> GetArchiveAsync(string id, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Docs/{**path}")]
	public Task<IActionResult> GetDocsAsync(string id, string version, string path, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[Authorize]
	[HttpDelete]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> DeleteAsync(string id, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
