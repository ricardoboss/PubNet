using Microsoft.EntityFrameworkCore;
using PubNet.Database;
using PubNet.Database.Models;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class PackageVersionAnalyzerTask : BaseWorkerTask
{
	public readonly string Package;
	public readonly string Version;

	private PubNetContext? _db;
	private ILogger<PackageVersionAnalyzerTask>? _logger;
	private WorkerTaskQueue? _taskQueue;

	public PackageVersionAnalyzerTask(string package, string version) : base($"{nameof(PackageVersionAnalyzerTask)} for {package} {version}")
	{
		Package = package;
		Version = version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		_db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();
		_logger ??= services.GetRequiredService<ILogger<PackageVersionAnalyzerTask>>();
		_taskQueue ??= services.GetRequiredService<WorkerTaskQueue>();

		var package = await _db.Packages
			.Include(p => p.Versions)
			.FirstOrDefaultAsync(p => p.Name == Package, cancellationToken);
		if (package is null)
		{
			_logger.LogError("Could not find package {PackageName} for pubspec.yaml analysis", Package);

			return WorkerTaskResult.Failed;
		}

		var version = package.Versions.FirstOrDefault(v => v.Version == Version);
		if (version is null)
		{
			_logger.LogError("Could not find package {PackageName} version {PackageVersion} for pubspec.yaml analysis", Package, Version);

			return WorkerTaskResult.Failed;
		}

		var analysis = await _db.PackageVersionAnalyses.FirstOrDefaultAsync(a => a.Version == version,
			cancellationToken);
		if (analysis is null) return await CreateAnalysis(version, _db, _taskQueue, _logger, cancellationToken);

		_logger.LogTrace("Updating existing analysis for {PackageName} {PackageVersion}", Package, Version);

		return await UpdateAnalysis(analysis, _db, _taskQueue, _logger, cancellationToken);
	}

	private async Task<WorkerTaskResult> CreateAnalysis(PackageVersion version, PubNetContext db, WorkerTaskQueue taskQueue, ILogger logger, CancellationToken cancellationToken = default)
	{
		try
		{
			var analysis = new PackageVersionAnalysis
			{
				Version = version,
			};

			db.PackageVersionAnalyses.Add(analysis);

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

	private async Task<WorkerTaskResult> UpdateAnalysis(PackageVersionAnalysis analysis, DbContext db, WorkerTaskQueue taskQueue, ILogger logger, CancellationToken cancellationToken = default)
	{
		try
		{
			if (analysis.IsComplete())
			{
				analysis.CompletedAtUtc = DateTimeOffset.Now.ToUniversalTime();

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

	private void EnqueueMissingAnalyses(PackageVersionAnalysis analysis, WorkerTaskQueue taskQueue)
	{
		if (analysis.ReadmeFound is null)
			taskQueue.Enqueue(new ReadmeAnalyzerTask(analysis));

		if (analysis.Formatted is null)
			taskQueue.Enqueue(new FormatAnalyzerTask(analysis));

		if (analysis.DocumentationLink is null)
			taskQueue.Enqueue(new DocumentationGeneratorTask(analysis));
	}
}
