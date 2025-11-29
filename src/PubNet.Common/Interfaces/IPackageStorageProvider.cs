namespace PubNet.Common.Interfaces;

public interface IPackageStorageProvider
{
	public Task DeletePackageAsync(string name, CancellationToken cancellationToken = default);

	public Task DeletePackageVersionAsync(string name, string version, CancellationToken cancellationToken = default);

	public Task<string> StoreArchiveAsync(string name, string version, Stream stream, CancellationToken cancellationToken = default);

	public Stream ReadArchiveAsync(string name, string version);

	public Task StoreDocsAsync(string name, string version, string tempFolder, CancellationToken cancellationToken = default);

	public Task<string> GetDocsPathAsync(string name, string version, CancellationToken cancellationToken = default);
}
