namespace PubNet.PackageStorage.Abstractions;

public interface IArchiveStorage
{
	public Task<bool> DeleteArchivesAsync(string author, CancellationToken cancellationToken = default);

	public Task<bool> DeleteArchivesAsync(string author, string name, CancellationToken cancellationToken = default);

	public Task<bool> DeleteArchivesAsync(string author, string name, string version, CancellationToken cancellationToken = default);

	public Task<string> StoreArchiveAsync(string author, string name, string version, Stream stream, CancellationToken cancellationToken = default);

	public Task<Stream> ReadArchiveAsync(string author, string name, string version, CancellationToken cancellationToken = default);
}
