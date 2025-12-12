using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Packages;
using PubNet.API.Services;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("authors")]
[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
public class AuthorsController(ILogger<AuthorsController> logger, PubNetContext db, PasswordManager passwordManager)
	: BaseController
{
	[HttpGet("")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorsResponseDto))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
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
			if (!limit.HasValue) return BadRequest(ErrorResponseDto.InvalidQuery);

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
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(string username, CancellationToken cancellationToken = default)
	{
		using (logger.BeginScope(new Dictionary<string, object>
		{
			["AuthorUsername"] = username,
		}))
		{
			var author = await db.Authors.FirstOrDefaultAsync(a => a.UserName == username, cancellationToken);

			return author is null ? NotFound() : Ok(AuthorDto.FromAuthor(author));
		}
	}

	[HttpPost("{username}/delete")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponseDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public async Task<IActionResult> Delete([FromRoute] string username, [FromBody] DeleteAuthorRequestDto dto,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		using (logger.BeginScope(new Dictionary<string, object>
		{
			["AuthorUsername"] = username,
		}))
		{
			if (username != author.UserName) return Unauthorized(ErrorResponseDto.UsernameMismatch);

			if (!await passwordManager.IsValid(db, author, dto.Password, cancellationToken))
				return Unauthorized(ErrorResponseDto.InvalidPasswordConfirmation);

			foreach (var authorPackage in db.Packages.Where(p => p.Author == author))
				authorPackage.Author = null;

			await db.SaveChangesAsync(cancellationToken);

			db.Authors.Remove(author);

			await db.SaveChangesAsync(cancellationToken);

			return Ok(new SuccessResponseDto(new($"Author '{username}' successfully deleted.")));
		}
	}

	[HttpGet("{username}/packages")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorPackagesResponseDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetPackages(string username, CancellationToken cancellationToken = default)
	{
		var author = await db.Authors
			.Where(a => a.UserName == username)
			.Include(a => a.Packages)
			.FirstOrDefaultAsync(cancellationToken);

		return author is null
			? NotFound()
			: Ok(new AuthorPackagesResponseDto(author.Packages.Select(PackageDto.FromPackage)));
	}

	[HttpPatch("{username}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponseDto))]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public async Task<IActionResult> Edit(string username, [FromBody] EditAuthorRequestDto dto,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		using (logger.BeginScope(new Dictionary<string, object>
		{
			["AuthorUsername"] = author.UserName,
		}))
		{
			if (username != author.UserName) return Unauthorized(ErrorResponseDto.UsernameMismatch);

			if (dto.Name is not null && author.Name != dto.Name) author.Name = dto.Name;

			if (string.IsNullOrWhiteSpace(dto.Website) && author.Website is not null)
				author.Website = null;
			else if (author.Website != dto.Website) author.Website = dto.Website;

			await db.SaveChangesAsync(cancellationToken);

			return NoContent();
		}
	}
}
