using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Packages.Dart.Spec;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/{name}")]
[Tags("Dart")]
public class DartPackagesByNameController : DartController
{
	[Authorize]
	[HttpPatch("Discontinue")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<DartSuccessDto> Discontinue(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	public Task<DartPackageIndexDto> GetAsync(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
