using Microsoft.EntityFrameworkCore;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class PackageVersionAnalyzerTask : BaseWorkerTask
{
	public readonly string Package;
	public readonly string Version;

	private PubNet2Context? maybeDb;
	private ILogger<PackageVersionAnalyzerTask>? maybeLogger;
	private WorkerTaskQueue? maybeTaskQueue;

	public PackageVersionAnalyzerTask(string package, string version) : base($"{nameof(PackageVersionAnalyzerTask)} for {package} {version}")
	{
		Package = package;
		Version = version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		maybeDb ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNet2Context>();
		maybeLogger ??= services.GetRequiredService<ILogger<PackageVersionAnalyzerTask>>();
		maybeTaskQueue ??= services.GetRequiredService<WorkerTaskQueue>();

		var package = await maybeDb.DartPackages
			.Include(p => p.Versions)
			.FirstOrDefaultAsync(p => p.Name == Package, cancellationToken);
		if (package is null)
		{
			maybeLogger.LogError("Could not find package {PackageName} for pubspec.yaml analysis", Package);

			return WorkerTaskResult.Failed;
		}

		var version = package.Versions.FirstOrDefault(v => v.Version == Version);
		if (version is null)
		{
			maybeLogger.LogError("Could not find package {PackageName} version {PackageVersion} for pubspec.yaml analysis", Package, Version);

			return WorkerTaskResult.Failed;
		}

		var analysis = await maybeDb.DartPackageVersionAnalyses.FirstOrDefaultAsync(a => a.PackageVersion == version,
			cancellationToken);
		if (analysis is null) return await CreateAnalysis(version, maybeDb, maybeTaskQueue, maybeLogger, cancellationToken);

		maybeLogger.LogTrace("Updating existing analysis for {PackageName} {PackageVersion}", Package, Version);

		return await UpdateAnalysis(analysis, maybeDb, maybeTaskQueue, maybeLogger, cancellationToken);
	}

	private async Task<WorkerTaskResult> CreateAnalysis(DartPackageVersion version, PubNet2Context db, WorkerTaskQueue taskQueue, ILogger logger, CancellationToken cancellationToken = default)
	{
		try
		{
			var analysis = new DartPackageVersionAnalysis
			{
				PackageVersion = version,
			};

			db.DartPackageVersionAnalyses.Add(analysis);

			await db.SaveChangesAsync(cancellationToken);
			await db.Entry(analysis).ReloadAsync(cancellationToken);

			EnqueueMissingAnalyses(analysis, taskQueue);

			return WorkerTaskResult.Done;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to save new analysis of package {PackageName} version {PackageVersion}", Package, Version);

			return WorkerTaskResult.FailedRecoverable;
		}
	}

	private async Task<WorkerTaskResult> UpdateAnalysis(DartPackageVersionAnalysis analysis, DbContext db, WorkerTaskQueue taskQueue, ILogger logger, CancellationToken cancellationToken = default)
	{
		try
		{
			if (analysis.IsComplete())
			{
				analysis.CompletedAt = DateTimeOffset.Now.ToUniversalTime();

				logger.LogTrace("Analysis for {PackageName} {PackageVersion} marked as completed", Package, Version);

				await db.SaveChangesAsync(cancellationToken);
			}
			else
			{
				EnqueueMissingAnalyses(analysis, taskQueue);
			}

			return WorkerTaskResult.Done;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to save changes to analysis of package {PackageName} version {PackageVersion}", Package, Version);

			return WorkerTaskResult.FailedRecoverable;
		}
	}

	private void EnqueueMissingAnalyses(DartPackageVersionAnalysis analysis, WorkerTaskQueue taskQueue)
	{
		if (analysis.ReadmeFound is null)
			taskQueue.Enqueue(new ReadmeAnalyzerTask(analysis));

		if (analysis.Formatted is null)
			taskQueue.Enqueue(new FormatAnalyzerTask(analysis));

		if (analysis.DocumentationGenerated is null)
			taskQueue.Enqueue(new DocumentationGeneratorTask(analysis));
	}
}
