using Microsoft.Extensions.FileProviders;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;
using PubNet.DocsStorage.Abstractions;
using PubNet.PackageStorage.Abstractions;
using PubNet.Storage.Utils.Abstractions.Archives;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class DocumentationGeneratorTask : BaseWorkerTask
{
	private readonly DartPackageVersionAnalysis analysis;
	private readonly string author;
	private readonly string package;
	private readonly string version;

	private ILogger<DocumentationGeneratorTask>? logger;
	private IArchiveStorage? archiveStorage;
	private IArchiveReader? archiveReader;
	private IDocsStorage? docsStorage;
	private DartCli? dart;
	private PubNetContext? db;

	public DocumentationGeneratorTask(DartPackageVersionAnalysis analysis) : base($"{nameof(DocumentationGeneratorTask)} for {analysis.PackageVersion.Package.Author.UserName}/{analysis.PackageVersion.Package.Name} {analysis.PackageVersion.Version}")
	{
		this.analysis = analysis;

		author = this.analysis.PackageVersion.Package.Author.UserName;
		package = this.analysis.PackageVersion.Package.Name;
		version = this.analysis.PackageVersion.Version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		logger ??= services.GetRequiredService<ILogger<DocumentationGeneratorTask>>();
		archiveStorage ??= services.GetRequiredService<IArchiveStorage>();
		archiveReader ??= services.GetRequiredService<IArchiveReader>();
		docsStorage ??= services.GetRequiredService<IDocsStorage>();
		dart ??= services.GetRequiredService<DartCli>();
		db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();

		await db.Entry(analysis).ReloadAsync(cancellationToken);

		if (analysis.DocumentationGenerated is not null) return WorkerTaskResult.Done;

		var workingDir = Path.Combine(Path.GetTempPath(), "PubNet", "Analysis", nameof(DocumentationGeneratorTask), package, version);

		logger.LogTrace("Running {TaskName} in {WorkingDirectory}", Name, workingDir);

		await using (var archiveStream = await archiveStorage.ReadArchiveAsync(author, package, version, cancellationToken))
		{
			archiveReader.ReadIntoDirectory(archiveStream, workingDir);
		}

		try
		{
			logger.LogTrace("Getting dependencies for documentation");

			var exitCode = await dart.InvokeDart("pub get", workingDir, cancellationToken);
			if (exitCode != 0)
			{
				logger.LogError("Failed to get dependencies for documentation (exit code {ExitCode})", exitCode);

				return WorkerTaskResult.Failed;
			}

			logger.LogTrace("Generating documentation for package {PackageName} version {PackageVersion}", package, version);

			exitCode = await dart.Doc(workingDir, cancellationToken);
			if (exitCode != 0)
			{
				logger.LogError("Process to generate documentation exited with non-zero exit code ({ExitCode})", exitCode);

				return WorkerTaskResult.Failed;
			}

			var docsFileProvider = new PhysicalFileProvider(Path.Combine(workingDir, "doc", "api"));
			await docsStorage.StoreDocsAsync("test", package, version, docsFileProvider, cancellationToken);

			analysis.DocumentationGenerated = true;

			await db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		finally
		{
			Directory.Delete(workingDir, true);
		}
	}
}
