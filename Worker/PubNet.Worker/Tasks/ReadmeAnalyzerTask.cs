using PubNet.Common.Utils;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;
using PubNet.PackageStorage.Abstractions;
using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public class ReadmeAnalyzerTask : BaseWorkerTask
{
	private readonly DartPackageVersionAnalysis _analysis;
	private readonly string _package;
	private readonly string _version;

	private ILogger<ReadmeAnalyzerTask>? _logger;
	private IArchiveStorage? _archiveStorage;
	private PubNetContext? _db;

	public ReadmeAnalyzerTask(DartPackageVersionAnalysis analysis) : base($"{nameof(ReadmeAnalyzerTask)} for {analysis.PackageVersion.Package.Name} {analysis.PackageVersion.Version}")
	{
		_analysis = analysis;

		_package = _analysis.PackageVersion.Package.Name;
		_version = _analysis.PackageVersion.Version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		_logger ??= services.GetRequiredService<ILogger<ReadmeAnalyzerTask>>();
		_archiveStorage ??= services.GetRequiredService<IArchiveStorage>();
		_db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();

		await _db.Entry(_analysis).ReloadAsync(cancellationToken);

		if (_analysis.ReadmeFound is not null) return WorkerTaskResult.Done;

		var workingDir = Path.Combine(Path.GetTempPath(), "PubNet", "Analysis", nameof(ReadmeAnalyzerTask), _package, _version);

		_logger.LogTrace("Running {AnalyzerName} analysis in {WorkingDirectory}", nameof(ReadmeAnalyzerTask), workingDir);

		// FIXME: author
		await using (var archiveStream = await _archiveStorage.ReadArchiveAsync("test", _package, _version, cancellationToken))
		{
			ArchiveHelper.UnpackInto(archiveStream, workingDir);
		}

		try
		{
			_logger.LogTrace("Looking for a README file in package {PackageName} version {PackageVersion}", _package,
				_version);

			var readmePath = await PathHelper.GetCaseInsensitivePath(workingDir, "readme.md", cancellationToken);
			if (readmePath is null || !File.Exists(readmePath))
			{
				_analysis.ReadmeFound = false;

				await _db.SaveChangesAsync(cancellationToken);

				return WorkerTaskResult.Done;
			}

			_analysis.ReadmeFound = true;
			_analysis.ReadmeText = await File.ReadAllTextAsync(readmePath, cancellationToken);

			await _db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		finally
		{
			Directory.Delete(workingDir, true);
		}
	}
}
