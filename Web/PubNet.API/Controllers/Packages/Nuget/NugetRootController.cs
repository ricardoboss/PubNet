using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget")]
[Tags("Nuget")]
public class NugetRootController : NugetController
{
	[HttpGet("index.json")]
	public Task<IActionResult> GetServiceIndexAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Autocomplete")]
	public Task<NugetAutocompleteResultDto> AutocompleteAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("vulnerabilities.json")]
	public Task<IActionResult> GetVulnerabilitiesAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Search")]
	public Task<NugetSearchResultDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[Authorize]
	[HttpPut("Publish")]
	public Task<IActionResult> PublishAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
