using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;
using PubNet.PackageStorage.Abstractions;
using PubNet.Storage.Utils.Abstractions.Archives;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class FormatAnalyzerTask : BaseWorkerTask
{
	private readonly DartPackageVersionAnalysis analysis;
	private readonly string author;
	private readonly string package;
	private readonly string version;

	private ILogger<FormatAnalyzerTask>? logger;
	private IArchiveStorage? archiveStorage;
	private IArchiveReader? archiveReader;
	private DartCli? dart;
	private PubNetContext? db;

	public FormatAnalyzerTask(DartPackageVersionAnalysis analysis) : base($"{nameof(FormatAnalyzerTask)} for {analysis.PackageVersion.Package.Author.UserName}/{analysis.PackageVersion.Package.Name} {analysis.PackageVersion.Version}")
	{
		this.analysis = analysis;

		author = this.analysis.PackageVersion.Package.Author.UserName;
		package = this.analysis.PackageVersion.Package.Name;
		version = this.analysis.PackageVersion.Version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		logger ??= services.GetRequiredService<ILogger<FormatAnalyzerTask>>();
		archiveStorage ??= services.GetRequiredService<IArchiveStorage>();
		archiveReader ??= services.GetRequiredService<IArchiveReader>();
		dart ??= services.GetRequiredService<DartCli>();
		db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();

		await db.Entry(analysis).ReloadAsync(cancellationToken);

		if (analysis.Formatted is not null) return WorkerTaskResult.Done;

		var workingDir = Path.Combine(Path.GetTempPath(), "PubNet", "Analysis", nameof(FormatAnalyzerTask), package, version);

		logger.LogTrace("Running {AnalyzerName} analysis in {WorkingDirectory}", nameof(FormatAnalyzerTask), workingDir);

		await using (var archiveStream = await archiveStorage.ReadArchiveAsync(author, package, version, cancellationToken))
		{
			archiveReader.ReadIntoDirectory(archiveStream, workingDir);
		}

		try
		{
			logger.LogTrace("Check if package {PackageName} version {PackageVersion} is formatted", package, version);

			var exitCode = await dart.Format("lib", workingDir, cancellationToken);

			analysis.Formatted = exitCode == 0;

			await db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		finally
		{
			Directory.Delete(workingDir, true);
		}
	}
}
