using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Interfaces;

namespace PubNet.API.Services;

public class LocalPackageStorageProvider : IPackageStorageProvider
{
    private readonly IConfiguration _configuration;

    public LocalPackageStorageProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetArchivePath(string name, string version)
    {
        var path = Path.Combine(
            _configuration.GetRequiredSection("PackageStorage:Path").Value!,
            name,
            version,
            "archive.tar.gz"
        );

        return Path.GetFullPath(path);
    }

    /// <inheritdoc />
    public async Task<string> StoreArchive(string name, string version, Stream stream)
    {
        var path = GetArchivePath(name, version);

        if (!File.Exists(path))
        {
            var parent = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Cannot get parent directory for archive path");
            Directory.CreateDirectory(parent);
        }

        await using (var fileStream = File.OpenWrite(path))
            await stream.CopyToAsync(fileStream);

        await using (var fileStream = File.OpenRead(path))
        using (var hash = SHA256.Create())
            return Convert.ToHexString(await hash.ComputeHashAsync(fileStream));
    }

    /// <inheritdoc />
    public FileResult ReadArchive(string name, string version)
    {
        var path = GetArchivePath(name, version);

        if (!File.Exists(path))
            throw new FileNotFoundException("Archive file not found.", path);

        return new PhysicalFileResult(path, "application/octet-stream")
        {
            FileDownloadName = $"{name}-{version}.tar.gz",
        };
    }
}