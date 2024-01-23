using PubNet.Common.Utils;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;
using PubNet.PackageStorage.Abstractions;
using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public class ReadmeAnalyzerTask : BaseWorkerTask
{
	private readonly DartPackageVersionAnalysis analysis;
	private readonly string package;
	private readonly string version;

	private ILogger<ReadmeAnalyzerTask>? logger;
	private IArchiveStorage? archiveStorage;
	private PubNetContext? db;

	public ReadmeAnalyzerTask(DartPackageVersionAnalysis analysis) : base($"{nameof(ReadmeAnalyzerTask)} for {analysis.PackageVersion.Package.Name} {analysis.PackageVersion.Version}")
	{
		this.analysis = analysis;

		package = this.analysis.PackageVersion.Package.Name;
		version = this.analysis.PackageVersion.Version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		logger ??= services.GetRequiredService<ILogger<ReadmeAnalyzerTask>>();
		archiveStorage ??= services.GetRequiredService<IArchiveStorage>();
		db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();

		await db.Entry(analysis).ReloadAsync(cancellationToken);

		if (analysis.ReadmeFound is not null) return WorkerTaskResult.Done;

		var workingDir = Path.Combine(Path.GetTempPath(), "PubNet", "Analysis", nameof(ReadmeAnalyzerTask), package, version);

		logger.LogTrace("Running {AnalyzerName} analysis in {WorkingDirectory}", nameof(ReadmeAnalyzerTask), workingDir);

		// FIXME: author
		await using (var archiveStream = await archiveStorage.ReadArchiveAsync("test", package, version, cancellationToken))
		{
			ArchiveHelper.UnpackInto(archiveStream, workingDir);
		}

		try
		{
			logger.LogTrace("Looking for a README file in package {PackageName} version {PackageVersion}", package,
				version);

			var readmePath = await PathHelper.GetCaseInsensitivePath(workingDir, "readme.md", cancellationToken);
			if (readmePath is null || !File.Exists(readmePath))
			{
				analysis.ReadmeFound = false;

				await db.SaveChangesAsync(cancellationToken);

				return WorkerTaskResult.Done;
			}

			analysis.ReadmeFound = true;
			analysis.ReadmeText = await File.ReadAllTextAsync(readmePath, cancellationToken);

			await db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		finally
		{
			Directory.Delete(workingDir, true);
		}
	}
}
