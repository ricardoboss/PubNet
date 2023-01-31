using System.Security.Cryptography;
using PubNet.API.Interfaces;

namespace PubNet.API.Services;

public class LocalPackageStorageProvider : IPackageStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocalPackageStorageProvider> _logger;

    public LocalPackageStorageProvider(IConfiguration configuration, ILogger<LocalPackageStorageProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string GetStorageBasePath() => _configuration.GetRequiredSection("PackageStorage:Path").Value!;

    private string GetPackageBasePath(string name) => Path.Combine(GetStorageBasePath(), name);

    private string GetPackageVersionBasePath(string name, string version) =>
        Path.Combine(GetPackageBasePath(name), version);

    private string GetPackageVersionArchivePath(string name, string version)
    {
        var path = Path.Combine(GetPackageVersionBasePath(name, version), "archive.tar.gz");

        return Path.GetFullPath(path);
    }

    /// <inheritdoc />
    public async Task<string> StoreArchive(string name, string version, Stream stream, CancellationToken cancellationToken = default)
    {
        var path = GetPackageVersionArchivePath(name, version);

        _logger.LogDebug("Storing package archive for {PackageName} {PackageVersion} at {Path}", name, version, path);

        if (!File.Exists(path))
        {
            var parent = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Cannot get parent directory for archive path");
            Directory.CreateDirectory(parent);
        }

        await using (var fileStream = File.OpenWrite(path))
            await stream.CopyToAsync(fileStream, cancellationToken);

        await using (var fileStream = File.OpenRead(path))
        using (var hash = SHA256.Create())
            return Convert.ToHexString(await hash.ComputeHashAsync(fileStream, cancellationToken));
    }

    /// <inheritdoc />
    public Stream ReadArchive(string name, string version)
    {
        var path = GetPackageVersionArchivePath(name, version);

        if (!File.Exists(path))
            throw new FileNotFoundException("Archive file not found.", path);

        return File.OpenRead(path);
    }

    /// <inheritdoc />
    public async Task<string> StoreDocs(string name, string version, string tempFolder, CancellationToken cancellationToken = default)
    {
        var destination = await GetDocsPath(name, version, cancellationToken);

        _logger.LogDebug("Storing package documentation for {PackageName} {PackageVersion} at {Path}", name, version, destination);

        if (Directory.Exists(destination))
        {
            _logger.LogDebug("Removing old package documentation");

            Directory.Delete(destination, true);
        }

        Directory.Move(tempFolder, destination);

        return destination;
    }

    /// <inheritdoc />
    public Task<string> GetDocsPath(string name, string version, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = Path.Combine(GetPackageVersionBasePath(name, version), "docs");

        return Task.FromResult(Path.GetFullPath(path));
    }
}
