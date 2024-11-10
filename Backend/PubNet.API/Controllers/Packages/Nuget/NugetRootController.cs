using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Abstractions.Packages.Nuget;
using PubNet.API.Attributes;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.API.DTO.Packages.Nuget.Spec;
using PubNet.Auth;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget")]
[Tags("Nuget")]
public class NugetRootController(INugetServiceIndexProvider serviceIndexProvider, INugetPackageDao packageDao) : NugetController
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
	[Authorize, RequireAnyScope(Scopes.Packages.Nuget.Search, Scopes.Packages.Search)]
	[ProducesResponseType<NugetSearchResultDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<NugetSearchResultDto> SearchJsonAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		var list = await packageDao.SearchAsync(q, skip, take, cancellationToken);

		return NugetSearchResultDto.MapFrom(list);
	}

	[HttpGet("search")]
	[Authorize, RequireAnyScope(Scopes.Packages.Nuget.Search, Scopes.Packages.Search)]
	[ProducesResponseType<NugetPackageListDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<NugetPackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		return await packageDao.SearchAsync(q, skip, take, cancellationToken);
	}
}
