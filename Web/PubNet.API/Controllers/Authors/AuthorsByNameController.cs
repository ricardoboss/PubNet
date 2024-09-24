using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Attributes;
using PubNet.API.DTO;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.Web;

namespace PubNet.API.Controllers.Authors;

[Route("Authors/{username}")]
[Tags("Authors")]
public class AuthorsByNameController(IAuthorDao authorDao, IDartPackageArchiveProvider archiveProvider)
	: AuthorsController
{
	[HttpGet]
	[ProducesResponseType<AuthorDto>(200)]
	[ProducesResponseType<GenericErrorDto>(404)]
	public async Task<IActionResult> GetAuthorByUserNameAsync(string username,
		CancellationToken cancellationToken = default)
	{
		var author = await authorDao.TryFindByUsernameAsync(username, cancellationToken);
		if (author is null)
			return NotFound(new GenericErrorDto
			{
				Error = new()
				{
					Code = "author-not-found",
					Message = "Author not found",
				},
			});

		return Ok(AuthorDto.MapFrom(author));
	}

	[HttpGet("Packages/Dart")]
	public Task<DartPackageListDto> GetAuthorDartPackagesAsync(string username,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Packages/Nuget")]
	public Task<NugetPackageListDto> GetAuthorNugetPackagesAsync(string username,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Packages")]
	public async Task<PackageListCollectionDto> GetAuthorPackagesAsync(string username,
		CancellationToken cancellationToken = default)
	{
		var author = await authorDao.TryFindByUsernameAsync(username, cancellationToken);
		if (author is null)
			return new()
			{
				Dart = new()
				{
					TotalHits = 0,
					Packages = Array.Empty<DartPackageDto>(),
				},
				Nuget = new()
				{
					TotalHits = 0,
					Packages = Array.Empty<NugetPackageDto>(),
				},
			};

		var dartPackageDtos =
			author.DartPackages.Select(p => DartPackageDto.MapFrom(p, archiveProvider.GetArchiveUriAndHash));

		var nugetPackageDtos =
			author.NugetPackages.Select(p => NugetPackageDto.MapFrom(p));

		return new()
		{
			Dart = new()
			{
				TotalHits = author.DartPackages.Count,
				Packages = dartPackageDtos,
			},
			Nuget = new()
			{
				TotalHits = author.NugetPackages.Count,
				Packages = nugetPackageDtos,
			},
		};
	}

	[HttpDelete]
	[Authorize, RequireAnyScope(Scopes.Authors.Delete.Self, Scopes.Authors.Delete.Any)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> DeleteAuthorAsync(string username, DeleteAuthorDto dto,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
