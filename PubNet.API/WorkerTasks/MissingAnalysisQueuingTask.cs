using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.Services;
using PubNet.Models;

namespace PubNet.API.WorkerTasks;

public class MissingAnalysisQueuingTask : BaseWorkerTask
{
    private ILogger<MissingAnalysisQueuingTask>? _logger;
    private PubNetContext? _db;
    private WorkerTaskQueue? _taskQueue;

    public override async Task<WorkerTaskResult> Invoke(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        _logger ??= services.GetRequiredService<ILogger<MissingAnalysisQueuingTask>>();
        _db ??= services.GetRequiredService<PubNetContext>();
        _taskQueue ??= services.GetRequiredService<WorkerTaskQueue>();

        await EnqueueMissingAnalyses(_db, _logger, _taskQueue, cancellationToken);
        await EnqueueIncompleteAnalyses(_db, _logger, _taskQueue, cancellationToken);

        return WorkerTaskResult.Requeue;
    }

    private static async Task EnqueueMissingAnalyses(PubNetContext db, ILogger<MissingAnalysisQueuingTask> logger, WorkerTaskQueue taskQueue, CancellationToken cancellationToken = default)
    {
        var versionsWithoutAnalysis = await db.PackageVersions
            .Where(v => !db.PackageVersionAnalyses.Any(a => a.Version == v))
            .ToListAsync(cancellationToken: cancellationToken);

        if (versionsWithoutAnalysis.Count == 0)
        {
            logger.LogTrace("No package versions without analysis found");

            return;
        }

        logger.LogTrace("Found {Count} package versions without analysis", versionsWithoutAnalysis.Count);

        foreach (var packageVersion in versionsWithoutAnalysis)
        {
            taskQueue.Enqueue(CreateTaskFor(packageVersion));
        }
    }

    private static async Task EnqueueIncompleteAnalyses(PubNetContext db, ILogger<MissingAnalysisQueuingTask> logger, WorkerTaskQueue taskQueue, CancellationToken cancellationToken)
    {
        var incompleteAnalyses = await db.PackageVersionAnalyses
            .Where(a => a.Formatted == null || a.DocumentationLink == null)
            .ToListAsync(cancellationToken);

        if (incompleteAnalyses.Count == 0)
        {
            logger.LogTrace("No incomplete package version analyses found");

            return;
        }

        logger.LogTrace("Found {Count} incomplete package version analyses", incompleteAnalyses.Count);

        foreach (var analysis in incompleteAnalyses)
        {
            taskQueue.Enqueue(CreateTaskFor(analysis.Version));
        }
    }

    private static PubSpecAnalyzerTask CreateTaskFor(PackageVersion version)
    {
        return new(version.PackageName, version.Version);
    }

    public override bool RequeueOnException => true;
}