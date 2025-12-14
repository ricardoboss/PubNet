using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO.Authentication.Errors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Errors;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Common.Extensions;
using PubNet.Common.Interfaces;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Controllers;

[Authorize]
[ApiController]
[Route("packages")]
public class PackagesController(
	ILogger<PackagesController> logger,
	PubNetContext db,
	IPackageStorageProvider storageProvider,
	PubDevPackageProvider pubDevPackageProvider
) : BaseController
{
	private readonly FileExtensionContentTypeProvider _fileTypeProvider = new()
	{
		Mappings =
		{
			["dart"] = "text/plain",
		},
	};

	[HttpGet("")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchPackagesResponseDto))]
	[ProducesResponseType(PubNetStatusCodes.Status400BadRequest, Type = typeof(InvalidQueryErrorDto))]
	[ResponseCache(VaryByQueryKeys = ["q", "before", "limit"], Location = ResponseCacheLocation.Any,
		Duration = 60 * 60)]
	public IActionResult Get([FromQuery] string? q = null, [FromQuery] long? before = null,
		[FromQuery] int? limit = null)
	{
		const int maxLimit = 1000;

		IQueryable<Package> packages = db.Packages.OrderByDescending(p => p.Latest!.PublishedAtUtc);

		if (q != null) packages = packages.Where(p => p.Name.StartsWith(q));

		if (before.HasValue)
		{
			if (!limit.HasValue)
				return Error<InvalidQueryErrorDto>(PubNetStatusCodes.Status400BadRequest, "When before is set, a limit must also be set");

			var publishedAtUpperLimit = DateTimeOffset.FromUnixTimeMilliseconds(before.Value);

			packages = packages.Where(p => p.Latest!.PublishedAtUtc < publishedAtUpperLimit);
		}

		if (limit.HasValue)
		{
			var resultLimit = Math.Min(limit.Value, maxLimit);

			packages = packages.Take(resultLimit);
		}

		return Ok(new SearchPackagesResponseDto(packages.Include(p => p.Author).ToList().Select(p =>
			new SearchResultPackageDto(p.Name, p.ReplacedBy, p.IsDiscontinued, p.Author?.UserName, p.Latest?.Version,
				p.Latest?.PublishedAtUtc))));
	}

	[HttpGet("{name}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(PackageNotFoundErrorDto))]
	[ResponseCache(VaryByQueryKeys = ["includeAuthor"], Location = ResponseCacheLocation.Any, Duration = 60 * 10)]
	public async Task<IActionResult> GetByName(string name, [FromQuery] bool includeAuthor = false,
		CancellationToken cancellationToken = default)
	{
		var packageQuery = db.Packages
			.Include(p => p.Versions)
			.Where(p => p.Name == name);

		if (includeAuthor)
			packageQuery = packageQuery.Include(p => p.Author);

		var package = await packageQuery.FirstOrDefaultAsync(cancellationToken);

		PackageDto dto;
		if (package is not null)
		{
			dto = PackageDto.FromPackage(package);
		}
		else
		{
			var pubDevPackage = await pubDevPackageProvider.TryGetPackage(name, cancellationToken);
			if (pubDevPackage is null)
				return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package not found: " + name);

			dto = pubDevPackage;
		}

		return Ok(dto);
	}

	[HttpPatch("{name}/discontinue")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageDto))]
	[ProducesResponseType(PubNetStatusCodes.Status403Forbidden, Type = typeof(ForbiddenErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(PackageNotFoundErrorDto))]
	public async Task<IActionResult> DiscontinueByName(string name, [FromBody] SetDiscontinuedDto dto,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		var package = await db.Packages
			.Where(p => p.Name == name)
			.Include(p => p.Versions)
			.FirstOrDefaultAsync(cancellationToken);

		if (package is null)
			return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package not found: " + name);

		if (author.Id != package.AuthorId)
			return Error<ForbiddenErrorDto>(PubNetStatusCodes.Status403Forbidden, "You don't own this package");

		package.IsDiscontinued = true;
		package.ReplacedBy = dto.Replacement;

		await db.SaveChangesAsync(cancellationToken);
		await db.Entry(package).ReloadAsync(cancellationToken);

		return Ok(PackageDto.FromPackage(package));
	}

	[HttpDelete("{name}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(PubNetStatusCodes.Status403Forbidden, Type = typeof(ForbiddenErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(PackageNotFoundErrorDto))]
	public async Task<IActionResult> DeleteByName(string name, [FromServices] ApplicationRequestContext context,
		CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		var package = await db.Packages
			.Where(p => p.Name == name)
			.Include(p => p.Versions)
			.FirstOrDefaultAsync(cancellationToken);

		if (package is null)
			return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package not found: " + name);

		if (author.Id != package.AuthorId)
			return Error<ForbiddenErrorDto>(PubNetStatusCodes.Status403Forbidden, "You don't own this package");

		// decouple analyses from versions
		await db.PackageVersions
			.Where(v => v.PackageName == name)
			.ForEachAsync(v => v.Analysis = null, cancellationToken);
		await db.SaveChangesAsync(cancellationToken);

		// remove decoupled analyses
		db.PackageVersionAnalyses.RemoveRange(
			db.PackageVersionAnalyses
				.Include(a => a.Version)
				.Where(a => package.Versions.Any(v => v == a.Version))
		);
		await db.SaveChangesAsync(cancellationToken);

		// remove reference to latest version
		package.Latest = null;
		await db.SaveChangesAsync(cancellationToken);

		// remove package versions
		db.PackageVersions.RemoveRange(package.Versions);
		await db.SaveChangesAsync(cancellationToken);

		// finally, delete the package itself
		db.Packages.Remove(package);
		await db.SaveChangesAsync(cancellationToken);

		try
		{
			await storageProvider.DeletePackageAsync(name, cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to delete package from storage");
		}

		return NoContent();
	}

	[HttpGet("{name}/versions/{version}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageVersionDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(PackageNotFoundErrorDto))]
	[ResponseCache(Duration = 60 * 10, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersion(string name, string version,
		CancellationToken cancellationToken = default)
	{
		var package = await db.Packages
			.Where(p => p.Name == name)
			.Include(p => p.Versions)
			.Include(nameof(Package.Versions) + "." + nameof(PackageVersion.Analysis))
			.FirstOrDefaultAsync(cancellationToken);

		PackageVersionDto dto;
		var packageVersion = package?.Versions.FirstOrDefault(v => v.Version == version);
		if (packageVersion is not null)
		{
			dto = PackageVersionDto.FromPackageVersion(packageVersion);
		}
		else
		{
			var pubDevPackageVersion = await pubDevPackageProvider.TryGetVersion(name, version, cancellationToken);
			if (pubDevPackageVersion is null)
				return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package not found: " + name);

			dto = pubDevPackageVersion;
		}

		Response.Headers.ContentType = "application/vnd.pub.v2+json";
		return Ok(dto);
	}

	[HttpGet("{name}/versions/{version}/analysis")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageVersionAnalysisDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(PackageNotFoundErrorDto))]
	[ResponseCache(VaryByQueryKeys = ["includeReadme"], Duration = 60 * 60, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersionAnalysis(string name, string version,
		[FromQuery] bool includeReadme = false, CancellationToken cancellationToken = default)
	{
		var package = await db.Packages
			.Where(p => p.Name == name)
			.Include(p => p.Versions)
			.Include(nameof(Package.Versions) + "." + nameof(PackageVersion.Analysis))
			.FirstOrDefaultAsync(cancellationToken);

		var packageVersionAnalysis = package?.Versions.FirstOrDefault(v => v.Version == version)?.Analysis;
		if (packageVersionAnalysis is null)
			return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package, version or analysis not found: " + name + " version " + version);

		return Ok(PackageVersionAnalysisDto.FromPackageVersionAnalysis(packageVersionAnalysis, includeReadme));
	}

	[HttpPatch("{name}/versions/{version}/retract")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(PubNetStatusCodes.Status403Forbidden, Type = typeof(ForbiddenErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(PackageNotFoundErrorDto))]
	public async Task<IActionResult> RetractVersion(string name, string version,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		var package = await db.Packages
			.Where(p => p.Name == name)
			.Include(p => p.Versions)
			.FirstOrDefaultAsync(cancellationToken);
		if (package is null)
			return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package not found: " + name);

		if (author.Id != package.AuthorId)
			return Error<ForbiddenErrorDto>(PubNetStatusCodes.Status403Forbidden, "You don't own this package");

		var packageVersion = package.Versions.FirstOrDefault(v => v.Version == version);
		if (packageVersion is null)
			return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package version not found: " + name + " version " + version);

		if (package.Latest == packageVersion)
		{
			if (package.Versions.Count > 1)
			{
				var newLatestPackage = package.Versions
					.Where(v => v.Version != version && v.Retracted == false)
					.MaxBy(v => v.PublishedAtUtc);

				package.Latest = newLatestPackage;
			}
			else
			{
				package.Latest = null;
			}

			await db.SaveChangesAsync(cancellationToken);
		}

		packageVersion.Retracted = true;
		await db.SaveChangesAsync(cancellationToken);

		return NoContent();
	}

	[HttpDelete("{name}/versions/{version}")]
	[ProducesResponseType(PubNetStatusCodes.Status403Forbidden, Type = typeof(ForbiddenErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(PackageNotFoundErrorDto))]
	public async Task<IActionResult> DeleteVersion(string name, string version,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		using (logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
			["AuthorUsername"] = author.UserName,
		}))
		{
			var package = await db.Packages
				.Where(p => p.Name == name)
				.Include(p => p.Versions)
				.Include(nameof(Package.Versions) + "." + nameof(PackageVersion.Analysis))
				.FirstOrDefaultAsync(cancellationToken);
			if (package is null)
				return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package not found: " + name);

			if (author.Id != package.AuthorId)
				return Error<ForbiddenErrorDto>(PubNetStatusCodes.Status403Forbidden, "You don't own this package");

			var packageVersion = package.Versions.FirstOrDefault(v => v.Version == version);
			if (packageVersion is null)
				return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package version not found: " + name + " version " + version);

			if (package.Latest == packageVersion)
			{
				if (package.Versions.Count > 1)
				{
					var newLatestPackage = package.Versions
						.Where(v => v.Version != version && v.Retracted == false)
						.MaxBy(v => v.PublishedAtUtc);

					package.Latest = newLatestPackage;
				}
				else
				{
					package.Latest = null;
				}

				await db.SaveChangesAsync(cancellationToken);
			}

			if (packageVersion.Analysis is { } analysis)
			{
				db.PackageVersionAnalyses.Remove(analysis);
				await db.SaveChangesAsync(cancellationToken);
			}

			db.PackageVersions.Remove(packageVersion);
			await db.SaveChangesAsync(cancellationToken);

			try
			{
				await storageProvider.DeletePackageVersionAsync(name, version, cancellationToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Failed to delete package version from storage");
			}

			return NoContent();
		}
	}

	[HttpGet("{name}/versions/{version}.tar.gz")]
	[Produces("application/tar+gzip")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status302Found)]
	[ProducesResponseType(typeof(PackageNotFoundErrorDto), PubNetStatusCodes.Status404NotFound, "application/json")]
	[ResponseCache(Duration = 60 * 10, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersionArchive(string name, string version,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var archiveEntry = await storageProvider.GetArchiveAsync(name, version, cancellationToken);
			var archiveStream = archiveEntry.OpenRead();

			Response.RegisterForDisposeAsync(archiveStream);

			return new FileStreamResult(archiveStream, "application/tar+gzip")
			{
				FileDownloadName = $"{name}-{version}.tar.gz",
			};
		}
		catch (Exception ex)
		{
			if (ex is FileNotFoundException)
			{
				var pubDevPackageVersion = await pubDevPackageProvider.TryGetVersion(name, version, cancellationToken);
				if (pubDevPackageVersion is not null)
					return Redirect(pubDevPackageVersion.ArchiveUrl);
			}

			logger.LogError(ex, "Error reading archive for package \"{Package}\" version \"{Version}\"", name, version);

			return Error<PackageNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound, "Package not found: " + name);
		}
	}

	[AllowAnonymous]
	[HttpGet("{name}/versions/{version}/docs/")]
	[ProducesResponseType(StatusCodes.Status302Found)]
	public IActionResult GetVersionDocsIndex(string name, string version)
	{
		return RedirectToAction("GetVersionDocsFile", new { name, version, path = "index.html" });
	}

	[AllowAnonymous]
	[HttpGet("{name}/versions/{version}/docs/{**path}")]
	[Produces("text/html", "text/css", "application/javascript", "application/json", "application/octet-stream")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(object))]
	[ResponseCache(Duration = 60 * 10, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersionDocsFile(string name, string version, string path,
		CancellationToken cancellationToken = default)
	{
		var docsContainer = await storageProvider.GetDocsAsync(name, version, cancellationToken);

		var requestedEntry = docsContainer.GetRelativeEntry(path);
		if (requestedEntry is IFileContainer)
		{
			return RedirectToAction("GetVersionDocsFile", "Packages", new
			{
				name,
				version,
				path = path.TrimEnd('/') + "/index.html",
			});
		}

		IFileEntry? requestedFile = null;
		if (requestedEntry is IFileEntry existingFile)
			requestedFile = existingFile;

		if (requestedFile == null)
		{
			var notFoundEntry = docsContainer.GetChildEntry("__404error.html");
			if (notFoundEntry is not IFileEntry notFoundFile)
				return NotFound();

			requestedFile = notFoundFile;
		}

		if (!_fileTypeProvider.TryGetContentType(requestedFile.Name, out var contentType))
			contentType = "application/octet-stream";

		var contentStream = requestedFile.OpenRead();
		Response.RegisterForDisposeAsync(contentStream);

		return new FileStreamResult(contentStream, contentType);
	}

	[HttpGet("versions/new")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadEndpointDataDto))]
	public async Task<IActionResult> VersionsNew([FromServices] IUploadEndpointGenerator uploadEndpointGenerator,
		[FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, db, cancellationToken);

		var data = await uploadEndpointGenerator.GenerateUploadEndpointData(Request, author, cancellationToken);

		return Ok(data);
	}
}
