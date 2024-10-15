namespace PubNet.BlobStorage.Abstractions.Args;

public interface IPutBlobArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }

	string BlobName { get; }

	string ContentType { get; }

	Task<Stream> GetContentStreamAsync(CancellationToken cancellationToken = default);
}
