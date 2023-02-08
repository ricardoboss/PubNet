using Microsoft.EntityFrameworkCore;
using PubNet.Common.Interfaces;
using PubNet.Common.Utils;
using PubNet.Database;
using PubNet.Database.Models;
using PubNet.Worker.Models;
using PubNet.Worker.Services;

namespace PubNet.Worker.Tasks;

public class PubSpecAnalyzerTask : BaseWorkerTask
{
	public readonly string Package;
	public readonly string Version;
	private DartCli? _dart;

	private PubNetContext? _db;
	private ILogger<PubSpecAnalyzerTask>? _logger;
	private IPackageStorageProvider? _storageProvider;

	public PubSpecAnalyzerTask(string package, string version) : base($"{nameof(PubSpecAnalyzerTask)} for {package} {version}")
	{
		Package = package;
		Version = version;
	}

	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		_db ??= services.CreateAsyncScope().ServiceProvider.GetRequiredService<PubNetContext>();
		_logger ??= services.GetRequiredService<ILogger<PubSpecAnalyzerTask>>();
		_storageProvider ??= services.GetRequiredService<IPackageStorageProvider>();
		_dart ??= services.GetRequiredService<DartCli>();

		using (_logger.BeginScope(new Dictionary<string, string>
		{
			{ "PackageName", Package },
			{ "PackageVersion", Version },
		}))
		{
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
			if (analysis is null) return await CreateAnalysis(version, _storageProvider, _dart, _db, _logger, cancellationToken);

			_logger.LogTrace("Updating existing analysis for {PackageName} {PackageVersion}", Package, Version);

			return await UpdateAnalysis(analysis, _storageProvider, _dart, _db, _logger, cancellationToken);
		}
	}

	private async Task<WorkerTaskResult> CreateAnalysis(PackageVersion version, IPackageStorageProvider storageProvider, DartCli dart, PubNetContext db, ILogger<PubSpecAnalyzerTask> logger, CancellationToken cancellationToken = default)
	{
		var analysis = new PackageVersionAnalysis
		{
			Version = version,
		};

		// TODO: run any initial analysis

		db.PackageVersionAnalyses.Add(analysis);
		await db.SaveChangesAsync(cancellationToken);

		return await UpdateAnalysis(analysis, storageProvider, dart, db, logger, cancellationToken);
	}

	private async Task<WorkerTaskResult> UpdateAnalysis(PackageVersionAnalysis analysis, IPackageStorageProvider storageProvider, DartCli dart, PubNetContext db, ILogger<PubSpecAnalyzerTask> logger, CancellationToken cancellationToken = default)
	{
		var workingDir = Path.Combine(Path.GetTempPath(), "PubNetAnalysis", Package, Version);

		logger.LogTrace("Running analysis in {WorkingDirectory}", workingDir);

		await using (var archiveStream = storageProvider.ReadArchive(Package, Version))
		{
			ArchiveHelper.UnpackInto(archiveStream, workingDir);
		}

		try
		{
			await CheckFormatted(logger, analysis, dart, workingDir, cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error checking if package {PackageName} version {PackageVersion} is formatted", Package, Version);
		}

		try
		{
			await CheckDocsGenerated(logger, analysis, dart, workingDir, storageProvider, cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error generating the documentation for package {PackageName} version {PackageVersion}", Package, Version);
		}

		try
		{
			await CheckReadmeFound(logger, analysis, workingDir, cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error checking if package {PackageName} version {PackageVersion} has a README.md", Package, Version);
		}

		try
		{
			if (analysis.IsComplete())
				analysis.CompletedAtUtc = DateTimeOffset.Now.ToUniversalTime();

			await db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to save changes to analysis of package {PackageName} version {PackageVersion}", Package, Version);

			return WorkerTaskResult.FailedRecoverable;
		}
		finally
		{
			logger.LogTrace("Cleaning up working directory {WorkingDirectory}", workingDir);

			Directory.Delete(workingDir, true);
		}
	}

	private async Task CheckFormatted(ILogger logger, PackageVersionAnalysis analysis, DartCli dart, string workingDir, CancellationToken cancellationToken = default)
	{
		if (analysis.Formatted is not null) return;

		logger.LogTrace("Check if package {PackageName} version {PackageVersion} is formatted", Package, Version);

		var exitCode = await dart.Format("lib", workingDir, cancellationToken);

		analysis.Formatted = exitCode == 0;
	}

	private async Task CheckDocsGenerated(ILogger logger, PackageVersionAnalysis analysis, DartCli dart, string workingDir, IPackageStorageProvider storageProvider, CancellationToken cancellationToken = default)
	{
		if (analysis.DocumentationLink is not null) return;

		logger.LogTrace("Getting dependencies for documentation");

		var exitCode = await dart.InvokeDart("pub get", workingDir, cancellationToken);
		if (exitCode != 0)
		{
			logger.LogError("Failed to get dependencies for documentation (exit code {ExitCode})", exitCode);

			return;
		}

		logger.LogTrace("Generating documentation for package {PackageName} version {PackageVersion}", Package, Version);

		exitCode = await dart.Doc(workingDir, cancellationToken);
		if (exitCode != 0)
		{
			logger.LogError("Process to generate documentation exited with non-zero exit code ({ExitCode})", exitCode);

			return;
		}

		var apiDocPath = Path.Combine(workingDir, "doc", "api");
		await storageProvider.StoreDocs(Package, Version, apiDocPath, cancellationToken);

		analysis.DocumentationLink = $"/packages/{Package}/versions/{Version}/docs/";
	}

	private async Task CheckReadmeFound(ILogger logger, PackageVersionAnalysis analysis, string workingDir, CancellationToken cancellationToken = default)
	{
		if (analysis.ReadmeFound is not null) return;

		logger.LogTrace("Looking for a README file in package {PackageName} version {PackageVersion}", Package, Version);

		var readmePath = await PathHelper.GetCaseInsensitivePath(workingDir, "readme.md", cancellationToken);
		if (readmePath is null || !File.Exists(readmePath))
		{
			analysis.ReadmeFound = false;

			return;
		}

		analysis.ReadmeFound = true;
		analysis.ReadmeText = await File.ReadAllTextAsync(readmePath, cancellationToken);
	}
}
