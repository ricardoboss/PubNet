using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Attributes;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.Web;

namespace PubNet.API.Controllers.Authors;

[Route("Authors/{username}")]
[Tags("Authors")]
public class AuthorsByNameController : AuthorsController
{
	[HttpGet]
	public Task<AuthorDto?> GetAuthorByUserNameAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Packages/Dart")]
	public Task<DartPackageListDto> GetAuthorDartPackagesAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Packages/Nuget")]
	public Task<NugetPackageListDto> GetAuthorNugetPackagesAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Packages")]
	public Task<PackageListDto> GetAuthorPackagesAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpDelete]
	[Authorize, RequireAnyScope(Scopes.Authors.Delete.Self, Scopes.Authors.Delete.Any)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> DeleteAuthorAsync(string username, DeleteAuthorDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
