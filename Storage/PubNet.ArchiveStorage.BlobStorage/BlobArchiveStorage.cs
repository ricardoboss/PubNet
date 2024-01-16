using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions;
using PubNet.PackageStorage.Abstractions;

namespace PubNet.ArchiveStorage.BlobStorage;

public class BlobArchiveStorage(IBlobStorage blobStorage) : IArchiveStorage
{
	/// <inheritdoc />
	public Task<bool> DeleteArchivesAsync(string author, CancellationToken cancellationToken = default)
	{
		return blobStorage.DeleteBucket().WithBucketName(author).RunAsync(cancellationToken);
	}

	private static string GetArchiveName(string name, string version) => $"{name}.v{version}";

	/// <inheritdoc />
	public async Task<bool> DeleteArchivesAsync(string author, string name, CancellationToken cancellationToken = default)
	{
		var anyDeleted = false;
		await foreach (var blob in blobStorage.ListBlobs().WithBucketName(author).WithPattern($"{name}.v*").RunAsync(cancellationToken))
		{
			await blobStorage.DeleteBlob()
				.WithBucketName(author)
				.WithBlobName(blob.BlobName)
				.RunAsync(cancellationToken);

			anyDeleted = true;
		}

		return anyDeleted;
	}

	/// <inheritdoc />
	public Task<bool> DeleteArchivesAsync(string author, string name, string version, CancellationToken cancellationToken = default)
	{
		return blobStorage.DeleteBlob()
			.WithBucketName(author)
			.WithBlobName(GetArchiveName(name, version))
			.RunAsync(cancellationToken);
	}

	/// <inheritdoc />
	public Task<string> StoreArchiveAsync(string author, string name, string version, Stream stream, CancellationToken cancellationToken = default)
	{
		return blobStorage.PutBlob()
			.WithBucketName(author)
			.WithBlobName(GetArchiveName(name, version))
			.WithContentType("application/octet-stream")
			.WithContentStream(stream)
			.RunAsync(cancellationToken);
	}

	/// <inheritdoc />
	public async Task<Stream> ReadArchiveAsync(string author, string name, string version, CancellationToken cancellationToken = default)
	{
		var item = await blobStorage.GetBlob().WithBucketName(author).WithBlobName(GetArchiveName(name, version)).RunAsync(cancellationToken);

		return await item.OpenReadAsync(cancellationToken);
	}
}
