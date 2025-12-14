using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.DTO.Authentication.Errors;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Authors.Errors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Errors;
using PubNet.API.Services;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Controllers;

[Authorize]
[ApiController]
[Route("authors")]
[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
public class AuthorsController(PubNetContext db, PasswordManager passwordManager)
	: BaseController
{
	[HttpGet("")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorsResponseDto))]
	[ProducesResponseType(PubNetStatusCodes.Status400BadRequest, Type = typeof(InvalidQueryErrorDto))]
	[ResponseCache(VaryByQueryKeys = ["q", "before", "limit"], Location = ResponseCacheLocation.Any,
		Duration = 3600)]
	public IActionResult GetAll([FromQuery] string? q = null, [FromQuery] long? before = null,
		[FromQuery] int? limit = null)
	{
		const int maxLimit = 1000;

		IQueryable<Author> packages = db.Authors
				.Where(a => !a.Inactive)
				.OrderByDescending(p => p.RegisteredAtUtc)
			;

		if (q != null) packages = packages.Where(a => a.UserName.StartsWith(q));

		if (before.HasValue)
		{
			if (!limit.HasValue)
				return Error<InvalidQueryErrorDto>(PubNetStatusCodes.Status400BadRequest);

			var publishedAtUpperLimit = DateTimeOffset.FromUnixTimeMilliseconds(before.Value);

			packages = packages.Where(p => p.RegisteredAtUtc < publishedAtUpperLimit);
		}

		if (limit.HasValue)
		{
			var resultLimit = Math.Min(limit.Value, maxLimit);

			packages = packages.Take(resultLimit);
		}

		return Ok(new AuthorsResponseDto(
			packages.Select(a => new SearchResultAuthorDto(a.UserName, a.Name, a.Packages.Count, a.RegisteredAtUtc))));
	}

	[HttpGet("{username}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(AuthorNotFoundErrorDto))]
	public async Task<IActionResult> Get(string username, CancellationToken cancellationToken = default)
	{
		var author = await db.Authors.FirstOrDefaultAsync(a => a.UserName == username, cancellationToken);

		return author is null
			? Error<AuthorNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Author not found: " + username)
			: Ok(AuthorDto.FromAuthor(author));
	}

	[HttpPost("{username}/delete")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessMessageDto))]
	[ProducesResponseType(PubNetStatusCodes.Status403Forbidden, Type = typeof(ForbiddenErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status461InvalidPassword, Type = typeof(InvalidPasswordErrorDto))]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public async Task<IActionResult> Delete([FromRoute] string username, [FromBody] DeleteAuthorRequestDto dto,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		if (username != author.UserName)
			return Error<ForbiddenErrorDto>(PubNetStatusCodes.Status403Forbidden);

		if (!await passwordManager.IsValid(db, author, dto.Password, cancellationToken))
			return Error<InvalidPasswordErrorDto>(PubNetStatusCodes.Status461InvalidPassword);

		foreach (var authorPackage in db.Packages.Where(p => p.Author == author))
			authorPackage.Author = null;

		await db.SaveChangesAsync(cancellationToken);

		db.Authors.Remove(author);

		await db.SaveChangesAsync(cancellationToken);

		return Ok(new SuccessMessageDto
		{
			Success = new()
			{
				Code = "author-deleted",
				Message = $"Author '{username}' successfully deleted.",
			},
		});
	}

	[HttpGet("{username}/packages")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorPackagesResponseDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(AuthorNotFoundErrorDto))]
	public async Task<IActionResult> GetPackages(string username, CancellationToken cancellationToken = default)
	{
		var author = await db.Authors
			.Where(a => a.UserName == username)
			.Include(a => a.Packages)
			.FirstOrDefaultAsync(cancellationToken);

		return author is null
			? Error<AuthorNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Author not found: " + username)
			: Ok(new AuthorPackagesResponseDto(author.Packages.Select(PackageDto.FromPackage)));
	}

	[HttpPatch("{username}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status403Forbidden, Type = typeof(ForbiddenErrorDto))]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public async Task<IActionResult> Edit(string username, [FromBody] EditAuthorRequestDto dto,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		if (username != author.UserName)
			return Error<ForbiddenErrorDto>(PubNetStatusCodes.Status403Forbidden);

		if (dto.Name is not null && author.Name != dto.Name)
			author.Name = dto.Name;

		if (string.IsNullOrWhiteSpace(dto.Website) && author.Website is not null)
			author.Website = null;
		else if (author.Website != dto.Website)
			author.Website = dto.Website;

		await db.SaveChangesAsync(cancellationToken);

		await db.Entry(author).ReloadAsync(cancellationToken);

		return Ok(AuthorDto.FromAuthor(author));
	}
}
