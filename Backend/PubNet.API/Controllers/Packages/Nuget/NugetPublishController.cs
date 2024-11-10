using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Packaging;
using NuGet.Versioning;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.API.Services.Extensions;
using PubNet.Database.Entities.Nuget;
using PubNet.PackageStorage.Abstractions;

namespace PubNet.API.Controllers.Packages.Nuget;

[Route("Packages/Nuget/Publish")]
[Tags("Nuget")]
[Authorize]
[ProducesResponseType<NugetSuccessDto>(StatusCodes.Status201Created)]
[ProducesResponseType<NugetInvalidPackageErrorDto>(StatusCodes.Status400BadRequest)]
[ProducesResponseType<NugetPackageAlreadyExistsErrorDto>(StatusCodes.Status409Conflict)]
public class NugetPublishController(
	IOptions<PackageStorageOptions> storageOptions,
	INugetPackageDao packageDao,
	INugetPackageDmo packageDmo,
	IAuthProvider authProvider
) : NugetController
{
	[HttpPut]
	public async Task<IActionResult> PutAsync(IFormFile package, CancellationToken cancellationToken = default)
	{
		if (package.Length > storageOptions.Value.MaxFileSize)
			return StatusCode(
				StatusCodes.Status413PayloadTooLarge,
				new GenericErrorDto
				{
					Error = new GenericErrorContentDto
					{
						Code = "payload-too-large",
						Message = $"Maximum upload size is {storageOptions.Value.MaxFileSize} bytes",
					},
				}
			);

		var content = new byte[package.Length];
		await using (var stream = package.OpenReadStream())
		{
			var read = await stream.ReadAsync(content, cancellationToken);
			if (read != content.Length)
				throw new InvalidOperationException("Stream was not fully read");
		}

		using var packageStream = new MemoryStream(content);
		using var reader = new PackageArchiveReader(packageStream);

		var nuspec = reader.NuspecReader;

		var id = nuspec.GetId().ToNullIfEmpty();
		if (id is null)
			return StatusCode(
				StatusCodes.Status400BadRequest,
				new NugetInvalidPackageErrorDto
				{
					Message = "Package ID is missing from the package",
				}
			);

		var uploader = await authProvider.GetCurrentAuthorAsync(cancellationToken);

		var packageEntity = await packageDao.TryGetByPackageIdAsync(id, cancellationToken);
		NugetPackageVersion versionEntity;
		if (packageEntity is null)
		{
			versionEntity = await packageDmo.CreateAsync(uploader, content, cancellationToken);
		}
		else
		{
			var version = nuspec.GetVersion();

			if (packageEntity.Versions
				.Select(v => v.NuspecVersion)
				.Where(v => v is not null)
				.Cast<NuGetVersion>()
				.Any(v => v >= version))
			{
				return StatusCode(
					StatusCodes.Status409Conflict,
					new NugetPackageAlreadyExistsErrorDto
					{
						Message = $"Package {id} version {version} already exists",
					}
				);
			}

			versionEntity = await packageDmo.AddVersionAsync(packageEntity, content, cancellationToken);
		}

		return Created(versionEntity.Id.ToString(), new NugetSuccessDto
		{
			Success = $"Successfully published package {id} version {versionEntity.Version}",
		});
	}
}
