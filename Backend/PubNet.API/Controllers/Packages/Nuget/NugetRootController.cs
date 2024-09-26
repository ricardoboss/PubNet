﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Packages.Nuget;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget")]
[Tags("Nuget")]
public class NugetRootController(INugetServiceIndexProvider serviceIndexProvider) : NugetController
{
	[HttpGet("index.json")]
	public Task<NugetServiceIndexDto> GetServiceIndexAsync(CancellationToken cancellationToken = default)
	{
		return serviceIndexProvider.GetServiceIndexAsync(cancellationToken);
	}

	[HttpGet("autocomplete.json")]
	public Task<NugetAutocompleteResultDto> AutocompleteAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("vulnerabilities.json")]
	public Task<IEnumerable<NugetVulnerabilityIndexEntryDto>> GetVulnerabilitiesAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("search.json")]
	public Task<NugetSearchResultDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[Authorize]
	[HttpPut("Publish")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public Task<IActionResult> PublishAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}