﻿using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.Interfaces;
using PubNet.API.Models;
using PubNet.API.Services;
using PubNet.API.Utils;
using PubNet.Models;

namespace PubNet.API.WorkerTasks;

public class PubSpecAnalyzerTask : BaseWorkerTask
{
    public readonly string Package;
    public readonly string Version;

    private PubNetContext? _db;
    private ILogger<PubSpecAnalyzerTask>? _logger;
    private IPackageStorageProvider? _storageProvider;
    private DartCli? _dart;

    public PubSpecAnalyzerTask(string package, string version)
    {
        Package = package;
        Version = version;
    }

    public override string Name => $"{nameof(PubSpecAnalyzerTask)} for {Package} {Version}";

    public override async Task<WorkerTaskResult> Invoke(IServiceProvider services, CancellationToken cancellationToken = default)
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
            var package =
                await _db.Packages.FirstOrDefaultAsync(p => p.Name == Package, cancellationToken: cancellationToken);
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
                cancellationToken: cancellationToken);
            if (analysis is null)
            {
                return await CreateAnalysis(version, _storageProvider, _dart, _db, _logger, cancellationToken);
            }

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
            ArchiveHelper.UnpackInto(archiveStream, workingDir);

        if (analysis.Formatted is null)
        {
            logger.LogTrace("Check if package {PackageName} version {PackageVersion} is formatted", Package, Version);

            var exitCode = await dart.Format("lib", workingDir, cancellationToken);

            analysis.Formatted = exitCode == 0;
        }

        if (analysis.DocumentationLink is null)
        {
            logger.LogTrace("Generating documentation for package {PackageName} version {PackageVersion}", Package, Version);

            var exitCode = await dart.Doc("lib", workingDir, cancellationToken);
            if (exitCode != 0)
            {
                // TODO: handle possible error generating docs
            }
            else
            {
                // TODO: move {workingDir}/doc to package version storage
                analysis.DocumentationLink = $"https://localhost:44365/api/packages/{Package}/version/{Version}/docs/";
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return WorkerTaskResult.Done;
    }

    public override bool RequeueOnException => true;
}