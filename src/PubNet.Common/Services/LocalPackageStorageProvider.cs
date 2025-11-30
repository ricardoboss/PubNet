using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PubNet.Common.Extensions;
using PubNet.Common.Interfaces;
using PubNet.Common.Models;

namespace PubNet.Common.Services;

public class LocalPackageStorageProvider : IPackageStorageProvider
{
	private readonly ILogger<LocalPackageStorageProvider> _logger;
	private readonly IOptions<PackageStorageProviderOptions> _options;

	public LocalPackageStorageProvider(ILogger<LocalPackageStorageProvider> logger, IOptions<PackageStorageProviderOptions> options)
	{
		_logger = logger;
		_options = options;

		_logger.LogTrace("Local package storage base path is {StorageBasePath}", GetStorageBasePath());
	}

	/// <inheritdoc />
	public Task DeletePackageAsync(string name, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var path = GetPackageBasePath(name);

		_logger.LogInformation("Deleting package {PackageName} at {PackageBasePath}", name, path);

		Directory.Delete(path, true);

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public Task DeletePackageVersionAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var path = GetPackageVersionBasePath(name, version);

		_logger.LogInformation("Deleting package version {PackageName} {PackageVersion} at {PackageVersionBasePath}",
			name, version, path);

		Directory.Delete(path, true);

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public async Task<Sha256Hash> StoreArchiveAsync(string name, string version, IFileEntry archiveFile,
		CancellationToken cancellationToken = default)
	{
		var path = GetPackageVersionArchivePath(name, version);

		_logger.LogDebug("Storing package archive for {PackageName} {PackageVersion} at {Path}", name, version, path);

		if (!File.Exists(path))
		{
			var parent = Path.GetDirectoryName(path) ??
				throw new InvalidOperationException("Cannot get parent directory for archive path");
			Directory.CreateDirectory(parent);
		}

		await using (var outFileStream = File.OpenWrite(path))
		await using (var inFileStream = archiveFile.OpenRead())
		{
			await inFileStream.CopyToAsync(outFileStream, cancellationToken);
		}

		await using (var fileStream = File.OpenRead(path))
		using (var hash = SHA256.Create())
		{
			var rawHash = Convert.ToHexString(await hash.ComputeHashAsync(fileStream, cancellationToken));

			return Sha256Hash.From(rawHash);
		}
	}

	/// <inheritdoc />
	public Task<IFileEntry> GetArchiveAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		var path = GetPackageVersionArchivePath(name, version);
		var info = new FileInfo(path);

		if (!info.Exists)
			throw new FileNotFoundException("Archive file not found.", path);

		var entry = new FilesystemFileEntry(info);

		return Task.FromResult<IFileEntry>(entry);
	}

	/// <inheritdoc />
	public async Task StoreDocsAsync(string name, string version, IFileContainer docsContainer,
		CancellationToken cancellationToken = default)
	{
		var destinationPath = GetDocsPath(name, version);
		var destinationInfo = new DirectoryInfo(destinationPath);

		_logger.LogDebug("Storing package documentation for {PackageName} {PackageVersion} at {Path}", name, version,
			destinationPath);

		if (destinationInfo.Exists)
		{
			_logger.LogDebug("Removing old package documentation");

			destinationInfo.Delete(recursive: true);
		}

		await docsContainer.CopyToAsync(destinationInfo, cancellationToken);
	}

	/// <inheritdoc />
	public Task<IFileContainer> GetDocsAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var path = GetDocsPath(name, version);
		var info = new DirectoryInfo(path);
		var container = new DirectoryFileContainer(info);

		return Task.FromResult<IFileContainer>(container);
	}

	private string GetDocsPath(string name, string version) =>
		Path.Combine(GetPackageVersionBasePath(name, version), "docs");

	private string GetStorageBasePath()
	{
		var configuredPath = _options.Value.Path;
		return configuredPath is null
			? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PubNet",
				"packages")
			: Path.GetFullPath(configuredPath);
	}

	private string GetPackageBasePath(string name)
	{
		return Path.Combine(GetStorageBasePath(), name);
	}

	private string GetPackageVersionBasePath(string name, string version)
	{
		return Path.Combine(GetPackageBasePath(name), version);
	}

	private string GetPackageVersionArchivePath(string name, string version)
	{
		var path = Path.Combine(GetPackageVersionBasePath(name, version), "archive.tar.gz");

		return Path.GetFullPath(path);
	}
}
