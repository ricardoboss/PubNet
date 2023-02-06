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
			if (analysis.Formatted is null)
			{
				logger.LogTrace("Check if package {PackageName} version {PackageVersion} is formatted", Package, Version);

				var exitCode = await dart.Format("lib", workingDir, cancellationToken);

				analysis.Formatted = exitCode == 0;
			}

			if (analysis.DocumentationLink is null)
			{
				logger.LogTrace("Generating documentation for package {PackageName} version {PackageVersion}", Package, Version);

				var exitCode = await dart.Doc(workingDir, cancellationToken);
				if (exitCode != 0)
				{
					// TODO: handle possible error generating docs
				}
				else
				{
					// TODO: determine API url dynamically
					analysis.DocumentationLink = $"/packages/{Package}/versions/{Version}/docs/";

					var apiDocPath = Path.Combine(workingDir, "doc", "api");
					await storageProvider.StoreDocs(Package, Version, apiDocPath, cancellationToken);
				}
			}

			if (analysis.ReadmeFound is null)
			{
				logger.LogTrace("Looking for a README file in package {PackageName} version {PackageVersion}", Package, Version);

				var readmePath = await PathHelper.GetCaseInsensitivePath(workingDir, "readme.md", cancellationToken);
				if (readmePath is null || !File.Exists(readmePath))
				{
					analysis.ReadmeFound = false;
				}
				else
				{
					analysis.ReadmeFound = true;
					analysis.ReadmeText = await File.ReadAllTextAsync(readmePath, cancellationToken);
				}
			}

			if (analysis is not { ReadmeFound: null, DocumentationLink: null, Formatted: null })
				analysis.CompletedAtUtc = DateTimeOffset.Now.ToUniversalTime();

			await db.SaveChangesAsync(cancellationToken);

			return WorkerTaskResult.Done;
		}
		finally
		{
			Directory.Delete(workingDir, true);
		}
	}
}
