﻿using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.Services;

namespace PubNet.API.WorkerTasks;

public class CleanupOldPendingArchivesTask : BaseScheduledWorkerTask
{
    private ILogger<CleanupOldPendingArchivesTask>? _logger;
    private PubNetContext? _db;
    private IConfiguration? _configuration;

    /// <inheritdoc />
    public CleanupOldPendingArchivesTask(TimeSpan interval) : base(interval, DateTime.Now)
    {
    }

    /// <inheritdoc />
    protected override async Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        _db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();
        _logger ??= services.GetRequiredService<ILogger<CleanupOldPendingArchivesTask>>();
        _configuration ??= services.GetRequiredService<IConfiguration>();

        if (!TimeSpan.TryParse(_configuration.GetSection("PackageStorage:PendingMaxAge").Value ?? "7", out var maxAge))
        {
            throw new("Unable to parse PackageStorage:PendingMaxAge as a valid TimeSpan");
        }

        var uploadedAtLowerLimit = DateTimeOffset.UtcNow.Subtract(maxAge);

        var outdatedArchives = await _db.PendingArchives
            .Where(p => p.UploadedAtUtc < uploadedAtLowerLimit)
            .ToListAsync(cancellationToken: cancellationToken);

        _logger.LogTrace("Found {Count} outdated archive(s)", outdatedArchives.Count);

        foreach (var outdatedArchive in outdatedArchives)
        {
            var archivePath = outdatedArchive.ArchivePath;
            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            var unpackedArchivePath = outdatedArchive.UnpackedArchivePath;
            if (Directory.Exists(unpackedArchivePath))
            {
                Directory.Delete(unpackedArchivePath, true);
            }

            _db.PendingArchives.Remove(outdatedArchive);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return WorkerTaskResult.Requeue;
    }
}