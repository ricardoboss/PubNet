using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Attributes;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.API.Services.Extensions;
using PubNet.Auth;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart")]
[Tags("Dart")]
public class DartRootController(IAuthProvider authProvider, IDartPackageUploadService uploadService, IDartPackageDao packageDao) : DartController
{
	[HttpGet("Search")]
	[Authorize, RequireAnyScope(Scopes.Packages.Dart.Search, Scopes.Packages.Search)]
	[ProducesResponseType<DartPackageListDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<DartPackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		return await packageDao.SearchAsync(q, skip, take, cancellationToken);
	}

	[HttpGet("Versions/New")]
	[Authorize, RequireScope(Scopes.Packages.Dart.New)]
	[ProducesResponseType<DartArchiveUploadInformationDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<DartArchiveUploadInformationDto> CreateNewAsync(CancellationToken cancellationToken = default)
	{
		var token = await authProvider.GetCurrentTokenAsync(cancellationToken);

		return await uploadService.CreateNewAsync(token, cancellationToken);
	}
}
