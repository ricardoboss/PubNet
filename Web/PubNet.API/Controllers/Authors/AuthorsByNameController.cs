using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Authors;

[Route("Authors/{username}")]
[Tags("Authors")]
public class AuthorsByNameController : AuthorsController
{
	[HttpGet]
	public Task<IActionResult> GetAuthorByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Packages/Dart")]
	public Task<IActionResult> GetAuthorDartPackagesAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Packages/Nuget")]
	public Task<IActionResult> GetAuthorNugetPackagesAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
