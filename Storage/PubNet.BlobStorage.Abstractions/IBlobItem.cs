namespace PubNet.BlobStorage.Abstractions;

public interface IBlobItem
{
	string BucketName { get; }

	string BlobName { get; }

	string ContentType { get; }

	string ContentSha256 { get; }

	Task<Stream> OpenReadAsync(CancellationToken cancellationToken = default);
}
