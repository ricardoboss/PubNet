using Microsoft.EntityFrameworkCore;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions;
using PubNet.Database.Context;
using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public class CleanupOldPendingArchivesTask : BaseScheduledWorkerTask
{
	private IConfiguration? configuration;
	private PubNet2Context? db;
	private ILogger<CleanupOldPendingArchivesTask>? logger;

	/// <inheritdoc />
	public CleanupOldPendingArchivesTask(TimeSpan interval) : base(interval, DateTime.Now)
	{
	}

	/// <inheritdoc />
	protected override async Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services,
		CancellationToken cancellationToken = default)
	{
		db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNet2Context>();
		logger ??= services.GetRequiredService<ILogger<CleanupOldPendingArchivesTask>>();
		configuration ??= services.GetRequiredService<IConfiguration>();

		if (!TimeSpan.TryParse(configuration.GetSection("PackageStorage:PendingMaxAge").Value ?? "7d", out var maxAge))
			throw new("Unable to parse PackageStorage:PendingMaxAge as a valid TimeSpan");

		var uploadedAtLowerLimit = DateTimeOffset.UtcNow.Subtract(maxAge);

		var outdatedArchives = await db.DartPendingArchives
			.Where(p => p.UploadedAt < uploadedAtLowerLimit)
			.ToListAsync(cancellationToken);

		logger.LogTrace("Found {Count} outdated archive(s)", outdatedArchives.Count);

		foreach (var outdatedArchive in outdatedArchives)
		{
			if (!outdatedArchive.ArchivePath.StartsWith("blob://"))
			{
				logger.LogError("Archive {ArchiveId}: Not a blob archive ({ArchivePath})", outdatedArchive.Id,
					outdatedArchive.ArchivePath);

				continue;
			}

			var parts = outdatedArchive.ArchivePath["blob://".Length..].Split("/", 3);
			if (parts.Length != 3)
			{
				logger.LogError("Archive {ArchiveId}: Invalid blob archive path ({ArchivePath})",
					outdatedArchive.Id,
					outdatedArchive.ArchivePath);

				continue;
			}

			var (storageName, bucketName, blobName) = (parts[0], parts[1], parts[2]);

			var blobStorage = services.GetKeyedService<IBlobStorage>(storageName);
			if (blobStorage is null)
			{
				logger.LogError("Archive {ArchiveId}: Unknown blob storage type ({StorageName})", outdatedArchive.Id,
					storageName);

				continue;
			}

			var success = await blobStorage
				.DeleteBlob()
				.WithBucketName(bucketName)
				.WithBlobName(blobName)
				.RunAsync(cancellationToken);

			if (!success)
			{
				logger.LogError("Archive {ArchiveId}: Failed to delete blob archive ({ArchivePath})",
					outdatedArchive.Id, outdatedArchive.ArchivePath);

				continue;
			}

			logger.LogInformation("Archive {ArchiveId}: Deleted blob archive ({ArchivePath})",
				outdatedArchive.Id,
				outdatedArchive.ArchivePath);

			db.DartPendingArchives.Remove(outdatedArchive);
		}

		await db.SaveChangesAsync(cancellationToken);

		return WorkerTaskResult.Requeue;
	}
}
