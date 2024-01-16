using PubNet.Common.Utils;
using PubNet.Database;
using PubNet.Database.Models;
using PubNet.PackageStorage.Abstractions;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class FormatAnalyzerTask : BaseWorkerTask
{
	private readonly PackageVersionAnalysis _analysis;
	private readonly string _package;
	private readonly string _version;

	private ILogger<FormatAnalyzerTask>? _logger;
	private IArchiveStorage? _archiveStorage;
	private DartCli? _dart;
	private PubNetContext? _db;

	public FormatAnalyzerTask(PackageVersionAnalysis analysis) : base($"{nameof(FormatAnalyzerTask)} for {analysis.Version.PackageName} {analysis.Version.Version}")
	{
		_analysis = analysis;

		_package = _analysis.Version.PackageName;
		_version = _analysis.Version.Version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		_logger ??= services.GetRequiredService<ILogger<FormatAnalyzerTask>>();
		_archiveStorage ??= services.GetRequiredService<IArchiveStorage>();
		_dart ??= services.GetRequiredService<DartCli>();
		_db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();

		await _db.Entry(_analysis).ReloadAsync(cancellationToken);

		if (_analysis.Formatted is not null) return WorkerTaskResult.Done;

		var workingDir = Path.Combine(Path.GetTempPath(), "PubNet", "Analysis", nameof(FormatAnalyzerTask), _package, _version);

		_logger.LogTrace("Running {AnalyzerName} analysis in {WorkingDirectory}", nameof(FormatAnalyzerTask), workingDir);

		// FIXME: author
		await using (var archiveStream = await _archiveStorage.ReadArchiveAsync("test", _package, _version, cancellationToken))
		{
			ArchiveHelper.UnpackInto(archiveStream, workingDir);
		}

		try
		{
			_logger.LogTrace("Check if package {PackageName} version {PackageVersion} is formatted", _package, _version);

			var exitCode = await _dart.Format("lib", workingDir, cancellationToken);

			_analysis.Formatted = exitCode == 0;

			await _db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		finally
		{
			Directory.Delete(workingDir, true);
		}
	}
}
