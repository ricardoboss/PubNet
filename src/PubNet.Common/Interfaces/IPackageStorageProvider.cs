using PubNet.Common.Models;

namespace PubNet.Common.Interfaces;

public interface IPackageStorageProvider
{
	public Task DeletePackageAsync(string name, CancellationToken cancellationToken = default);

	public Task DeletePackageVersionAsync(string name, string version, CancellationToken cancellationToken = default);

	public Task<Sha256Hash> StoreArchiveAsync(string name, string version, IFileEntry archiveFile, CancellationToken cancellationToken = default);

	public Task<IFileEntry> GetArchiveAsync(string name, string version, CancellationToken cancellationToken = default);

	public Task StoreDocsAsync(string name, string version, IFileContainer docsContainer, CancellationToken cancellationToken = default);

	public Task<IFileContainer> GetDocsAsync(string name, string version, CancellationToken cancellationToken = default);
}
