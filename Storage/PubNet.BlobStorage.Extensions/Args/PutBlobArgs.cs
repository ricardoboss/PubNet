using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record PutBlobArgs(string BucketName, string BlobName, string ContentType, Stream ContentStream) : IPutBlobArgs
{
	/// <inheritdoc />
	public Task<Stream> GetContentStreamAsync(CancellationToken cancellationToken = default) => Task.FromResult(ContentStream);
}
