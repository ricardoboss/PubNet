using Microsoft.EntityFrameworkCore;
using PubNet.Database;
using PubNet.Database.Models;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class MissingPackageVersionAnalysisQueuingTask(TimeSpan interval)
	: BaseScheduledWorkerTask(interval, DateTime.Now)
{
	private PubNetContext? _db;
	private ILogger<MissingPackageVersionAnalysisQueuingTask>? _logger;
	private WorkerTaskQueue? _taskQueue;

	public override bool RequeueOnException => true;

	protected override async Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services,
		CancellationToken cancellationToken = default)
	{
		_logger ??= services.GetRequiredService<ILogger<MissingPackageVersionAnalysisQueuingTask>>();
		_db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();
		_taskQueue ??= services.GetRequiredService<WorkerTaskQueue>();

		var anyFailed = false;
		try
		{
			await EnqueueMissingAnalyses(_db, _logger, _taskQueue, cancellationToken);
		}
		catch (Exception e)
		{
			anyFailed = true;
			_logger.LogError(e, "Failed to enqueue missing analyses");
		}

		try
		{
			await EnqueueIncompleteAnalyses(_db, _logger, _taskQueue, cancellationToken);
		}
		catch (Exception e)
		{
			anyFailed = true;
			_logger.LogError(e, "Failed to enqueue incomplete analyses");
		}

		return anyFailed ? WorkerTaskResult.FailedRecoverable : WorkerTaskResult.Requeue;
	}

	private static async Task EnqueueMissingAnalyses(PubNetContext db, ILogger logger, WorkerTaskQueue taskQueue,
		CancellationToken cancellationToken = default)
	{
		var versionsWithoutAnalysis = (await db.PackageVersions
			.Where(v => !db.PackageVersionAnalyses.Any(a => a.Version == v))
			.ToListAsync(cancellationToken))
			.Where(v => !TaskQueueContainsTaskFor(taskQueue, v))
			.ToList();

		if (versionsWithoutAnalysis.Count == 0)
		{
			logger.LogTrace("No package versions without analysis found");

			return;
		}

		logger.LogTrace("Found {Count} package versions without analysis", versionsWithoutAnalysis.Count);

		foreach (var packageVersion in versionsWithoutAnalysis) taskQueue.Enqueue(CreateTaskFor(packageVersion));
	}

	private static async Task EnqueueIncompleteAnalyses(PubNetContext db, ILogger logger, WorkerTaskQueue taskQueue,
		CancellationToken cancellationToken = default)
	{
		var incompleteAnalyses = (await db.PackageVersionAnalyses
			.Where(a => a.CompletedAtUtc == null)
			.ToListAsync(cancellationToken))
			.Where(a => !TaskQueueContainsTaskFor(taskQueue, a.Version))
			.ToList();

		if (incompleteAnalyses.Count == 0)
		{
			logger.LogTrace("No incomplete package version analyses found");

			return;
		}

		logger.LogTrace("Found {Count} incomplete package version analyses", incompleteAnalyses.Count);

		foreach (var analysis in incompleteAnalyses) taskQueue.Enqueue(CreateTaskFor(analysis.Version));
	}

	private static PackageVersionAnalyzerTask CreateTaskFor(PackageVersion version)
	{
		return new(version.PackageName, version.Version);
	}

	private static bool TaskQueueContainsTaskFor(WorkerTaskQueue taskQueue, PackageVersion version)
	{
		return taskQueue.Tasks.Any(t =>
			t is PackageVersionAnalyzerTask analyzerTask &&
			analyzerTask.Package == version.PackageName &&
			analyzerTask.Version == version.Version);
	}
}
