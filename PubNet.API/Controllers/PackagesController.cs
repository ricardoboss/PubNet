using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Common.Interfaces;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Controllers;

[ApiController]
[Route("packages")]
public class PackagesController : BaseController
{
	private readonly PubNetContext _db;
	private readonly FileExtensionContentTypeProvider _fileTypeProvider;
	private readonly ILogger<PackagesController> _logger;
	private readonly PubDevPackageProvider _pubDevPackageProvider;
	private readonly IPackageStorageProvider _storageProvider;

	public PackagesController(ILogger<PackagesController> logger, PubNetContext db, IPackageStorageProvider storageProvider, PubDevPackageProvider pubDevPackageProvider)
	{
		_logger = logger;
		_db = db;
		_storageProvider = storageProvider;
		_fileTypeProvider = new()
		{
			Mappings =
			{
				["dart"] = "text/plain",
			},
		};
		_pubDevPackageProvider = pubDevPackageProvider;
	}

	[HttpGet("")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchPackagesResponse))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
	[ResponseCache(VaryByQueryKeys = new[] { "q", "before", "limit" }, Location = ResponseCacheLocation.Any, Duration = 60 * 60)]
	public IActionResult Get([FromQuery] string? q = null, [FromQuery] long? before = null, [FromQuery] int? limit = null)
	{
		const int maxLimit = 1000;

		IQueryable<Package> packages = _db.Packages.OrderByDescending(p => p.Latest!.PublishedAtUtc);

		if (q != null) packages = packages.Where(p => p.Name.StartsWith(q));

		if (before.HasValue)
		{
			if (!limit.HasValue) return BadRequest(ErrorResponse.InvalidQuery);

			var publishedAtUpperLimit = DateTimeOffset.FromUnixTimeMilliseconds(before.Value);

			packages = packages.Where(p => p.Latest!.PublishedAtUtc < publishedAtUpperLimit);
		}

		if (limit.HasValue)
		{
			var resultLimit = Math.Min(limit.Value, maxLimit);

			packages = packages.Take(resultLimit);
		}

		return Ok(new SearchPackagesResponse(packages.Include(p => p.Author).ToList().Select(p => new SearchResultPackage(p.Name, p.ReplacedBy, p.IsDiscontinued, p.Author?.UserName, p.Latest?.Version, p.Latest?.PublishedAtUtc))));
	}

	[HttpGet("{name}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ResponseCache(VaryByQueryKeys = new[] { "includeAuthor" }, Location = ResponseCacheLocation.Any, Duration = 60 * 10)]
	public async Task<IActionResult> GetByName(string name, [FromQuery] bool includeAuthor = false, CancellationToken cancellationToken = default)
	{
		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
		}))
		{
			var packageQuery = _db.Packages
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
				var pubDevPackage = await _pubDevPackageProvider.TryGetPackage(name, cancellationToken);
				if (pubDevPackage is null) return NotFound();

				dto = pubDevPackage;
			}

			return Ok(dto);
		}
	}

	[HttpPatch("{name}/discontinue")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DiscontinueByName(string name, [FromBody] SetDiscontinuedDto dto, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
			["AuthorUsername"] = author.UserName,
		}))
		{
			var package = await _db.Packages
				.Where(p => p.Name == name)
				.Include(p => p.Versions)
				.FirstOrDefaultAsync(cancellationToken);

			if (package is null) return NotFound();

			if (author.Id != package.AuthorId) return Unauthorized(ErrorResponse.PackageAuthorMismatch);

			package.IsDiscontinued = true;
			package.ReplacedBy = dto.Replacement;
			await _db.SaveChangesAsync(cancellationToken);

			return NoContent();
		}
	}

	[HttpDelete("{name}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteByName(string name, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
			["AuthorUsername"] = author.UserName,
		}))
		{
			var package = await _db.Packages
				.Where(p => p.Name == name)
				.Include(p => p.Versions)
				.FirstOrDefaultAsync(cancellationToken);

			if (package is null) return NotFound();

			if (author.Id != package.AuthorId) return Unauthorized(ErrorResponse.PackageAuthorMismatch);

			// decouple analyses from versions
			await _db.PackageVersions
				.Where(v => v.PackageName == name)
				.ForEachAsync(v => v.Analysis = null, cancellationToken);
			await _db.SaveChangesAsync(cancellationToken);

			// remove decoupled analyses
			_db.PackageVersionAnalyses.RemoveRange(
				_db.PackageVersionAnalyses
					.Include(a => a.Version)
					.Where(a => package.Versions.Any(v => v == a.Version))
			);
			await _db.SaveChangesAsync(cancellationToken);

			// remove reference to latest version
			package.Latest = null;
			await _db.SaveChangesAsync(cancellationToken);

			// remove package versions
			_db.PackageVersions.RemoveRange(package.Versions);
			await _db.SaveChangesAsync(cancellationToken);

			// finally, delete the package itself
			_db.Packages.Remove(package);
			await _db.SaveChangesAsync(cancellationToken);

			try
			{
				await _storageProvider.DeletePackage(name, cancellationToken);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to delete package from storage");
			}

			return NoContent();
		}
	}

	[HttpGet("{name}/versions/{version}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageVersionDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ResponseCache(Duration = 60 * 10, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersion(string name, string version, CancellationToken cancellationToken = default)
	{
		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
		}))
		{
			var package = await _db.Packages
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
				var pubDevPackageVersion = await _pubDevPackageProvider.TryGetVersion(name, version, cancellationToken);
				if (pubDevPackageVersion is null) return NotFound();

				dto = pubDevPackageVersion;
			}

			Response.Headers.ContentType = "application/vnd.pub.v2+json";
			return Ok(dto);
		}
	}

	[HttpGet("{name}/versions/{version}/analysis")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackageVersionAnalysisDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ResponseCache(VaryByQueryKeys = new [] { "includeReadme" }, Duration = 60 * 60, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersionAnalysis(string name, string version, [FromQuery] bool includeReadme = false, CancellationToken cancellationToken = default)
	{
		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
		}))
		{
			var package = await _db.Packages
				.Where(p => p.Name == name)
				.Include(p => p.Versions)
				.Include(nameof(Package.Versions) + "." + nameof(PackageVersion.Analysis))
				.FirstOrDefaultAsync(cancellationToken);

			var packageVersionAnalysis = package?.Versions.FirstOrDefault(v => v.Version == version)?.Analysis;
			if (packageVersionAnalysis is null) return NotFound();

			return Ok(PackageVersionAnalysisDto.FromPackageVersionAnalysis(packageVersionAnalysis, includeReadme));
		}
	}

	[HttpPatch("{name}/versions/{version}/retract")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> RetractVersion(string name, string version, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
			["AuthorUsername"] = author.UserName,
		}))
		{
			var package = await _db.Packages
				.Where(p => p.Name == name)
				.Include(p => p.Versions)
				.FirstOrDefaultAsync(cancellationToken);
			if (package is null) return NotFound();

			if (author.Id != package.AuthorId) return Unauthorized(ErrorResponse.PackageAuthorMismatch);

			var packageVersion = package.Versions.FirstOrDefault(v => v.Version == version);
			if (packageVersion is null) return NotFound();

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

				await _db.SaveChangesAsync(cancellationToken);
			}

			packageVersion.Retracted = true;
			await _db.SaveChangesAsync(cancellationToken);

			return NoContent();
		}
	}

	[HttpDelete("{name}/versions/{version}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteVersion(string name, string version, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["PackageName"] = name,
			["AuthorUsername"] = author.UserName,
		}))
		{
			var package = await _db.Packages
				.Where(p => p.Name == name)
				.Include(p => p.Versions)
				.Include(nameof(Package.Versions) + "." + nameof(PackageVersion.Analysis))
				.FirstOrDefaultAsync(cancellationToken);
			if (package is null) return NotFound();

			if (author.Id != package.AuthorId) return Unauthorized(ErrorResponse.PackageAuthorMismatch);

			var packageVersion = package.Versions.FirstOrDefault(v => v.Version == version);
			if (packageVersion is null) return NotFound();

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

				await _db.SaveChangesAsync(cancellationToken);
			}

			if (packageVersion.Analysis is { } analysis)
			{
				_db.PackageVersionAnalyses.Remove(analysis);
				await _db.SaveChangesAsync(cancellationToken);
			}

			_db.PackageVersions.Remove(packageVersion);
			await _db.SaveChangesAsync(cancellationToken);

			try
			{
				await _storageProvider.DeletePackageVersion(name, version, cancellationToken);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to delete package version from storage");
			}

			return NoContent();
		}
	}

	[HttpGet("{name}/versions/{version}.tar.gz")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ResponseCache(Duration = 60 * 10, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersionArchive(string name, string version, CancellationToken cancellationToken = default)
	{
		try
		{
			var stream = _storageProvider.ReadArchive(name, version);

			return new FileStreamResult(stream, "application/octet-stream")
			{
				FileDownloadName = $"{name}-{version}.tar.gz",
			};
		}
		catch (Exception ex)
		{
			if (ex is FileNotFoundException)
			{
				var pubDevPackageVersion = await _pubDevPackageProvider.TryGetVersion(name, version, cancellationToken);
				if (pubDevPackageVersion is not null)
					return Redirect(pubDevPackageVersion.ArchiveUrl);
			}

			_logger.LogError(ex, "Error reading archive for package \"{Package}\" version \"{Version}\"", name, version);

			return NotFound();
		}
	}

	[HttpGet("{name}/versions/{version}/docs/")]
	public IActionResult GetVersionDocsIndex(string name, string version)
	{
		return RedirectToAction("GetVersionDocsFile", new { name, version, path = "index.html" });
	}

	[HttpGet("{name}/versions/{version}/docs/{**path}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ResponseCache(Duration = 60 * 10, Location = ResponseCacheLocation.Any)]
	public async Task<IActionResult> GetVersionDocsFile(string name, string version, string path, CancellationToken cancellationToken = default)
	{
		var localPath = await _storageProvider.GetDocsPath(name, version, cancellationToken);

		var notFoundPage = Path.Combine(localPath, "__404error.html");
		if (!System.IO.File.Exists(notFoundPage))
			return NotFound();

		string content;

		var requestedFile = Path.Combine(localPath, path);
		if (!System.IO.File.Exists(requestedFile))
			content = await System.IO.File.ReadAllTextAsync(notFoundPage, cancellationToken);
		else
			content = await System.IO.File.ReadAllTextAsync(requestedFile, cancellationToken);

		if (!_fileTypeProvider.TryGetContentType(requestedFile, out var contentType))
			contentType = "application/octet-stream";

		return new ContentResult
		{
			Content = content,
			ContentType = contentType,
			StatusCode = StatusCodes.Status200OK,
		};
	}

	[HttpGet("versions/new")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadEndpointData))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
	public async Task<IActionResult> VersionsNew([FromServices] IUploadEndpointGenerator uploadEndpointGenerator, [FromServices] ApplicationRequestContext context, CancellationToken cancellationToken = default)
	{
		var author = await context.RequireAuthorAsync(User, _db, cancellationToken);

		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["AuthorUsername"] = author.UserName,
		}))
		{
			var data = await uploadEndpointGenerator.GenerateUploadEndpointData(Request, author, cancellationToken);

			return Ok(data);
		}
	}
}
