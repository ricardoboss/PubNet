using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Attributes;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.Auth;

namespace PubNet.API.Controllers.Authors;

[Route("Authors/{username}")]
[Authorize]
[Tags("Authors")]
public class AuthorsByNameController(IAuthorDao authorDao, IDartPackageArchiveProvider archiveProvider)
	: AuthorsController
{
	[HttpGet]
	[ProducesResponseType<AuthorDto>(200)]
	[ProducesResponseType<NotFoundErrorDto>(404)]
	public async Task<IActionResult> GetAuthorByUserNameAsync(string username,
		CancellationToken cancellationToken = default)
	{
		var author = await authorDao.TryFindByUsernameAsync(username, cancellationToken);
		if (author is null)
			return NotFoundDto("author-not-found", "Author not found");

		return Ok(AuthorDto.MapFrom(author));
	}

	[HttpGet("Packages/Dart")]
	[RequireAnyScope(Scopes.Packages.Dart.Search, Scopes.Packages.Search)]
	public async Task<DartPackageListDto> GetAuthorDartPackagesAsync(string username, string? q = null,
		int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		var author = await authorDao.TryFindByUsernameAsync(username, cancellationToken);
		if (author is null)
			return new DartPackageListDto
			{
				TotalHits = 0,
				Packages = Array.Empty<DartPackageDto>(),
			};

		var packages = author.DartPackages.AsQueryable();

		if (q is not null)
			packages = packages.Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase));

		var filteredCount = packages.Count();

		if (skip is not null)
			packages = packages.Skip(skip.Value);

		if (take is not null)
			packages = packages.Take(take.Value);

		var searchResults = packages.ToList();
		var packageDtos = searchResults.Select(p => DartPackageDto.MapFrom(p, archiveProvider.GetArchiveUriAndHash));

		return new DartPackageListDto
		{
			TotalHits = filteredCount,
			Packages = packageDtos,
		};
	}

	[HttpGet("Packages/Nuget")]
	[RequireAnyScope(Scopes.Packages.Nuget.Search, Scopes.Packages.Search)]
	public async Task<NugetPackageListDto> GetAuthorNugetPackagesAsync(string username, string? q = null,
		int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		var author = await authorDao.TryFindByUsernameAsync(username, cancellationToken);
		if (author is null)
			return new NugetPackageListDto
			{
				TotalHits = 0,
				Packages = Array.Empty<NugetPackageDto>(),
			};

		var packages = author.NugetPackages.AsQueryable();

		if (q is not null)
			packages = packages.Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase));

		var filteredCount = packages.Count();

		if (skip is not null)
			packages = packages.Skip(skip.Value);

		if (take is not null)
			packages = packages.Take(take.Value);

		var searchResults = packages.ToList();
		var packageDtos = searchResults.Select(p => NugetPackageDto.MapFrom(p));

		return new NugetPackageListDto
		{
			TotalHits = filteredCount,
			Packages = packageDtos,
		};
	}

	[HttpGet("Packages")]
	[RequireAnyScope(Scopes.Packages.Search)]
	public async Task<PackageListCollectionDto> GetAuthorPackagesAsync(string username,
		CancellationToken cancellationToken = default)
	{
		var author = await authorDao.TryFindByUsernameAsync(username, cancellationToken);
		if (author is null)
			return new PackageListCollectionDto
			{
				Dart = new DartPackageListDto
				{
					TotalHits = 0,
					Packages = Array.Empty<DartPackageDto>(),
				},
				Nuget = new NugetPackageListDto
				{
					TotalHits = 0,
					Packages = Array.Empty<NugetPackageDto>(),
				},
			};

		var dartPackageDtos =
			author.DartPackages.Select(p => DartPackageDto.MapFrom(p, archiveProvider.GetArchiveUriAndHash));

		var nugetPackageDtos =
			author.NugetPackages.Select(p => NugetPackageDto.MapFrom(p));

		return new PackageListCollectionDto
		{
			Dart = new DartPackageListDto
			{
				TotalHits = author.DartPackages.Count,
				Packages = dartPackageDtos,
			},
			Nuget = new NugetPackageListDto
			{
				TotalHits = author.NugetPackages.Count,
				Packages = nugetPackageDtos,
			},
		};
	}

	[HttpDelete]
	[RequireAnyScope(Scopes.Authors.Delete.Self, Scopes.Authors.Delete.Any)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> DeleteAuthorAsync(string username, DeleteAuthorDto dto,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
