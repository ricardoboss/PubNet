namespace PubNet.Common.Interfaces;

public interface IPackageStorageProvider
{
	public Task DeletePackage(string name, CancellationToken cancellationToken = default);

	public Task DeletePackageVersion(string name, string version, CancellationToken cancellationToken = default);

	public Task<string> StoreArchive(string name, string version, Stream stream, CancellationToken cancellationToken = default);

	public Stream ReadArchive(string name, string version);

	public Task StoreDocs(string name, string version, string tempFolder, CancellationToken cancellationToken = default);

	public Task<string> GetDocsPath(string name, string version, CancellationToken cancellationToken = default);
}
