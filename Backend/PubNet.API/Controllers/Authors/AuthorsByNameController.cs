using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Attributes;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
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
			return new()
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

		return new()
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
			return new()
			{
				Dart = new()
				{
					TotalHits = 0,
					Packages = Array.Empty<DartPackageDto>(),
				},
			};

		var dartPackageDtos =
			author.DartPackages.Select(p => DartPackageDto.MapFrom(p, archiveProvider.GetArchiveUriAndHash));

		return new()
		{
			Dart = new()
			{
				TotalHits = author.DartPackages.Count,
				Packages = dartPackageDtos,
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
