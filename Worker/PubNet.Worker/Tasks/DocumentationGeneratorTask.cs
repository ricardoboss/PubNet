using Microsoft.Extensions.FileProviders;
using PubNet.Common.Utils;
using PubNet.Database;
using PubNet.Database.Models;
using PubNet.DocsStorage.Abstractions;
using PubNet.PackageStorage.Abstractions;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class DocumentationGeneratorTask : BaseWorkerTask
{
	private readonly PackageVersionAnalysis _analysis;
	private readonly string _package;
	private readonly string _version;

	private ILogger<DocumentationGeneratorTask>? _logger;
	private IArchiveStorage? _archiveStorage;
	private IDocsStorage? _docsStorage;
	private DartCli? _dart;
	private PubNetContext? _db;

	public DocumentationGeneratorTask(PackageVersionAnalysis analysis) : base($"{nameof(DocumentationGeneratorTask)} for {analysis.Version.PackageName} {analysis.Version.Version}")
	{
		_analysis = analysis;
		_package = _analysis.Version.PackageName;
		_version = _analysis.Version.Version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		_logger ??= services.GetRequiredService<ILogger<DocumentationGeneratorTask>>();
		_archiveStorage ??= services.GetRequiredService<IArchiveStorage>();
		_docsStorage ??= services.GetRequiredService<IDocsStorage>();
		_dart ??= services.GetRequiredService<DartCli>();
		_db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();

		await _db.Entry(_analysis).ReloadAsync(cancellationToken);

		if (_analysis.DocumentationLink is not null) return WorkerTaskResult.Done;

		var workingDir = Path.Combine(Path.GetTempPath(), "PubNet", "Analysis", nameof(DocumentationGeneratorTask), _package, _version);

		_logger.LogTrace("Running {TaskName} in {WorkingDirectory}", Name, workingDir);

		// FIXME: get author
		await using (var archiveStream = await _archiveStorage.ReadArchiveAsync("test", _package, _version, cancellationToken))
		{
			ArchiveHelper.UnpackInto(archiveStream, workingDir);
		}

		try
		{
			_logger.LogTrace("Getting dependencies for documentation");

			var exitCode = await _dart.InvokeDart("pub get", workingDir, cancellationToken);
			if (exitCode != 0)
			{
				_logger.LogError("Failed to get dependencies for documentation (exit code {ExitCode})", exitCode);

				return WorkerTaskResult.Failed;
			}

			_logger.LogTrace("Generating documentation for package {PackageName} version {PackageVersion}", _package, _version);

			exitCode = await _dart.Doc(workingDir, cancellationToken);
			if (exitCode != 0)
			{
				_logger.LogError("Process to generate documentation exited with non-zero exit code ({ExitCode})", exitCode);

				return WorkerTaskResult.Failed;
			}

			var docsFileProvider = new PhysicalFileProvider(Path.Combine(workingDir, "doc", "api"));
			await _docsStorage.StoreDocsAsync("test", _package, _version, docsFileProvider, cancellationToken);

			_analysis.DocumentationLink = $"/packages/{_package}/versions/{_version}/docs/";

			await _db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		finally
		{
			Directory.Delete(workingDir, true);
		}
	}
}
