using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Nuget;

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
}
