using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Attributes;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.API.Services.Extensions;
using PubNet.Web;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart")]
[Tags("Dart")]
public class DartRootController(IAuthProvider authProvider, IDartPackageUploadService uploadService) : DartController
{
	[HttpGet("Versions/New")]
	[Authorize, RequireScope(Scopes.Dart.New)]
	public async Task<DartNewVersionDto> CreateNewAsync(CancellationToken cancellationToken = default)
	{
		var token = await authProvider.GetCurrentTokenAsync(cancellationToken);

		return await uploadService.CreateNewAsync(token, cancellationToken);
	}
}
