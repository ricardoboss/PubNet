using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;
using PubNet.PackageStorage.Abstractions;
using SignedUrl.Abstractions;

namespace PubNet.API.Controllers.Packages.Dart;

[Route("Packages/Dart/Storage")]
[Tags("Dart")]
public class DartStorageController(
	PubNetContext context,
	IBlobStorage blobStorage,
	IUrlSigner urlSigner,
	IDartPackageUploadService uploadService,
	IOptions<PackageStorageOptions> storageOptions,
	ILogger<DartStorageController> logger
) : DartController
{
	[HttpPost]
	[ProducesResponseType<string>(201)]
	[ProducesResponseType<GenericErrorDto>(400)]
	[ProducesResponseType<NotFoundErrorDto>(404)]
	[ProducesResponseType<GenericErrorDto>(411)]
	[ProducesResponseType<GenericErrorDto>(413)]
	public async Task<IActionResult> UploadAsync(CancellationToken cancellationToken = default)
	{
		const string bucketName = "dart";

		var size = Request.Headers.ContentLength;
		if (size is null)
			return StatusCode(StatusCodes.Status411LengthRequired, new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "length-required", Message = "Content-Length header is required",
				},
			});

		if (size > storageOptions.Value.MaxFileSize)
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

		var contentType = Request.Headers.ContentType.FirstOrDefault();
		switch (contentType?.Split(';')[0])
		{
			case null:
				return BadRequest(new GenericErrorDto
				{
					Error = new GenericErrorContentDto
					{
						Code = "missing-content-type",
						Message = "Content-Type is missing from the request",
					},
				});
			case "multipart/form-data":
				break;
			default:
				return BadRequest(new GenericErrorDto
				{
					Error = new GenericErrorContentDto
					{
						Code = "invalid-content-type",
						Message = $"Request Content-Type must be multipart/form-data but is {contentType}",
					},
				});
		}

		var packageFile = Request.Form.Files.FirstOrDefault(f => f.Name == "file");
		if (packageFile is null)
			return BadRequest(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "missing-file",
					Message = "File is missing from the request",
				},
			});

		if (packageFile.ContentType is not "application/octet-stream")
			return BadRequest(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "invalid-content-type",
					Message = "Package file Content-Type must be application/octet-stream",
				},
			});

		if (!Request.Form.ContainsKey("author-id"))
			return BadRequest(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "missing-author-id",
					Message = "Author ID is missing from the request",
				},
			});

		var authorId = Request.Form["author-id"].ToString();
		var author = await context.Authors.FindAsync([Guid.Parse(authorId)], cancellationToken);
		if (author is null)
			return NotFoundDto("author-not-found", "Author not found");

		logger.LogInformation("Uploading package file {Package} ({PackageSize} bytes) for author {Author}",
			packageFile.FileName, packageFile.Length, author.Id);

		var buffer = new byte[packageFile.Length];

		try
		{
			await packageFile.OpenReadStream().ReadExactlyAsync(buffer, cancellationToken);
		}
		catch (EndOfStreamException e)
		{
			logger.LogWarning(e, "Upload was incomplete");

			return BadRequest(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "incomplete-upload",
					Message = "Upload was incomplete",
				},
			});
		}

		var blobName = $"{Guid.NewGuid():N}.tar.gz";
		var blobAddress = $"blob://{blobStorage.Name}/{bucketName}/{blobName}";

		var contentHash = await blobStorage
			.PutBlob()
			.WithBucketName(bucketName)
			.WithBlobName(blobName)
			.WithContentType(packageFile.ContentType)
			.WithContent(buffer)
			.RunAsync(cancellationToken);

		logger.LogInformation("Stored package file with hash {ContentHash} at {BlobAddress}", contentHash, blobAddress);

		var pendingArchive = new DartPendingArchive
		{
			Uploader = author,
			UploadedAt = DateTimeOffset.UtcNow,
			ArchivePath = blobAddress,
			ArchiveHash = contentHash,
		};

		await context.DartPendingArchives.AddAsync(pendingArchive, cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(pendingArchive).ReloadAsync(cancellationToken);

		logger.LogInformation("Assigned pending ID {PendingId} to package file", pendingArchive.Id);

		var finalizeUrl = Url.ActionLink("Finalize", "DartStorage", new { pendingId = pendingArchive.Id });
		if (finalizeUrl is null)
			throw new InvalidOperationException("Could not generate URL for finalize endpoint");

		var signedUrl = urlSigner.Sign(finalizeUrl);

		return Created(signedUrl, contentHash);
	}

	[HttpGet("{pendingId}")]
	[ProducesResponseType<DartSuccessDto>(200)]
	[ProducesResponseType<GenericErrorDto>(400)]
	[ProducesResponseType<GenericErrorDto>(422)]
	public async Task<IActionResult> FinalizeAsync(string pendingId, CancellationToken cancellationToken = default)
	{
		var url = Request.GetDisplayUrl();
		if (!urlSigner.Validate(url))
			return BadRequest(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "invalid-signature",
					Message = "URL signature is invalid",
				},
			});

		if (!Guid.TryParse(pendingId, out var pendingGuid))
			return BadRequest(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "invalid-guid",
					Message = "Pending ID is not a valid GUID",
				},
			});

		var pendingArchive = await context.DartPendingArchives.FindAsync([pendingGuid], cancellationToken);
		if (pendingArchive is null)
			return UnprocessableEntity(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "pending-not-found",
					Message = "Pending archive not found",
				},
			});

		logger.LogInformation("Finalizing package file with pending ID {PendingId}", pendingArchive.Id);

		try
		{
			var finalizedVersion = await uploadService.FinalizeNewAsync(pendingArchive, cancellationToken);

			return Ok(new DartSuccessDto
			{
				Success = new DartMessageDto
				{
					Message = $"Successfully uploaded package version {finalizedVersion}",
				},
			});
		}
		catch (DartPackageVersionAlreadyExistsException e)
		{
			return Conflict(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "package-version-already-exists",
					Message = e.Message,
				},
			});
		}
		catch (DartPackageVersionOutdatedException e)
		{
			return Conflict(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "package-version-outdated",
					Message = e.Message,
				},
			});
		}
		catch (UnauthorizedAccessException e)
		{
			logger.LogWarning(e, "Caught author trying to upload a package when it wasn't allowed");

			return Conflict(new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "unauthorized-access",
					Message = e.Message,
				},
			});
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to finalize package");

			return StatusCode(StatusCodes.Status500InternalServerError, new GenericErrorDto
			{
				Error = new GenericErrorContentDto
				{
					Code = "internal-server-error",
					Message = $"Failed to finalize package: {e.Message}",
				},
			});
		}
	}
}
