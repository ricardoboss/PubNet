using PubNet.BlobStorage.Abstractions;

namespace PubNet.BlobStorage.LocalFileBlobStorage;

public record FileBlobItem(string BucketName, string BlobName, string ContentType, string ContentSha256, FileInfo File) : IBlobItem
{
	/// <inheritdoc />
	public Task<Stream> OpenReadAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.FromResult<Stream>(File.OpenRead());
	}
}
