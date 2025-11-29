using PubNet.Common.Interfaces;
using PubNet.Common.Utils;
using PubNet.Database;
using PubNet.Database.Models;
using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public class ReadmeAnalyzerTask : BaseWorkerTask
{
	private readonly PackageVersionAnalysis _analysis;
	private readonly string _package;
	private readonly string _version;

	private ILogger<ReadmeAnalyzerTask>? _logger;
	private IPackageStorageProvider? _storageProvider;
	private PubNetContext? _db;

	public ReadmeAnalyzerTask(PackageVersionAnalysis analysis) : base($"{nameof(ReadmeAnalyzerTask)} for {analysis.Version.PackageName} {analysis.Version.Version}")
	{
		_analysis = analysis;

		_package = _analysis.Version.PackageName;
		_version = _analysis.Version.Version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		_logger ??= services.GetRequiredService<ILogger<ReadmeAnalyzerTask>>();
		_storageProvider ??= services.GetRequiredService<IPackageStorageProvider>();
		_db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();

		await _db.Entry(_analysis).ReloadAsync(cancellationToken);

		if (_analysis.ReadmeFound is not null) return WorkerTaskResult.Done;

		var workingDir = Path.Combine(Path.GetTempPath(), "PubNet", "Analysis", nameof(ReadmeAnalyzerTask), _package, _version);

		_logger.LogTrace("Running {AnalyzerName} analysis in {WorkingDirectory}", nameof(ReadmeAnalyzerTask), workingDir);

		var archiveFile = await _storageProvider.GetArchiveAsync(_package, _version, cancellationToken);
		await using (var archiveStream = archiveFile.OpenRead())
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
