using Microsoft.EntityFrameworkCore;
using PubNet.Database.Context;
using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public class CleanupOldPendingArchivesTask : BaseScheduledWorkerTask
{
	private IConfiguration? configuration;
	private PubNetContext? db;
	private ILogger<CleanupOldPendingArchivesTask>? logger;

	/// <inheritdoc />
	public CleanupOldPendingArchivesTask(TimeSpan interval) : base(interval, DateTime.Now)
	{
	}

	/// <inheritdoc />
	protected override async Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services,
		CancellationToken cancellationToken = default)
	{
		db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();
		logger ??= services.GetRequiredService<ILogger<CleanupOldPendingArchivesTask>>();
		configuration ??= services.GetRequiredService<IConfiguration>();

		if (!TimeSpan.TryParse(configuration.GetSection("PackageStorage:PendingMaxAge").Value ?? "7", out var maxAge))
			throw new("Unable to parse PackageStorage:PendingMaxAge as a valid TimeSpan");

		var uploadedAtLowerLimit = DateTimeOffset.UtcNow.Subtract(maxAge);

		var outdatedArchives = await db.DartPendingArchives
			.Where(p => p.UploadedAt < uploadedAtLowerLimit)
			.ToListAsync(cancellationToken);

		logger.LogTrace("Found {Count} outdated archive(s)", outdatedArchives.Count);

		foreach (var outdatedArchive in outdatedArchives)
		{
			var archivePath = outdatedArchive.ArchivePath;
			if (File.Exists(archivePath)) File.Delete(archivePath);

			var unpackedArchivePath = outdatedArchive.UnpackedArchivePath;
			if (Directory.Exists(unpackedArchivePath)) Directory.Delete(unpackedArchivePath, true);

			db.DartPendingArchives.Remove(outdatedArchive);
		}

		await db.SaveChangesAsync(cancellationToken);

		return WorkerTaskResult.Requeue;
	}
}
