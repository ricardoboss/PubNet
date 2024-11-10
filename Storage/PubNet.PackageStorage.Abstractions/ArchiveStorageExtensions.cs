namespace PubNet.PackageStorage.Abstractions;

public static class ArchiveStorageExtensions
{
	public static async Task<string> StoreArchiveAsync(this IArchiveStorage storage, string author, string name,
		string version, byte[] content, CancellationToken cancellationToken = default)
	{
		using var archiveStream = new MemoryStream(content);

		return await storage.StoreArchiveAsync(author, name, version, archiveStream, cancellationToken);
	}
}
