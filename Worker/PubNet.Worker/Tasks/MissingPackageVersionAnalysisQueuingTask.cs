using Microsoft.EntityFrameworkCore;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class MissingPackageVersionAnalysisQueuingTask : BaseScheduledWorkerTask
{
	private PubNetContext? db;
	private ILogger<MissingPackageVersionAnalysisQueuingTask>? logger;
	private WorkerTaskQueue? taskQueue;

	public MissingPackageVersionAnalysisQueuingTask(TimeSpan interval) : base(interval, DateTime.Now)
	{
	}

	public override bool RequeueOnException => true;

	protected override async Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services,
		CancellationToken cancellationToken = default)
	{
		logger ??= services.GetRequiredService<ILogger<MissingPackageVersionAnalysisQueuingTask>>();
		db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();
		taskQueue ??= services.GetRequiredService<WorkerTaskQueue>();

		var anyFailed = false;
		try
		{
			await EnqueueMissingAnalyses(db, logger, taskQueue, cancellationToken);
		}
		catch (Exception e)
		{
			anyFailed = true;
			logger.LogError(e, "Failed to enqueue missing analyses");
		}

		try
		{
			await EnqueueIncompleteAnalyses(db, logger, taskQueue, cancellationToken);
		}
		catch (Exception e)
		{
			anyFailed = true;
			logger.LogError(e, "Failed to enqueue incomplete analyses");
		}

		return anyFailed ? WorkerTaskResult.FailedRecoverable : WorkerTaskResult.Requeue;
	}

	private static async Task EnqueueMissingAnalyses(PubNetContext db, ILogger logger, WorkerTaskQueue taskQueue,
		CancellationToken cancellationToken = default)
	{
		var versionsWithoutAnalysis = (await db.DartPackageVersions
				.Where(v => !db.DartPackageVersionAnalyses.Any(a => a.PackageVersion == v))
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
		var incompleteAnalyses = (await db.DartPackageVersionAnalyses
				.Where(a => a.CompletedAt == null)
				.Include(dartPackageVersionAnalysis => dartPackageVersionAnalysis.PackageVersion)
				.ToListAsync(cancellationToken))
			.Where(a => !TaskQueueContainsTaskFor(taskQueue, a.PackageVersion))
			.ToList();

		if (incompleteAnalyses.Count == 0)
		{
			logger.LogTrace("No incomplete package version analyses found");

			return;
		}

		logger.LogTrace("Found {Count} incomplete package version analyses", incompleteAnalyses.Count);

		foreach (var analysis in incompleteAnalyses) taskQueue.Enqueue(CreateTaskFor(analysis.PackageVersion));
	}

	private static PackageVersionAnalyzerTask CreateTaskFor(DartPackageVersion version)
	{
		return new(version.Package.Name, version.Version);
	}

	private static bool TaskQueueContainsTaskFor(WorkerTaskQueue taskQueue, DartPackageVersion version)
	{
		return taskQueue.Tasks.Any(t =>
			t is PackageVersionAnalyzerTask analyzerTask &&
			analyzerTask.Package == version.Package.Name &&
			analyzerTask.Version == version.Version);
	}
}
