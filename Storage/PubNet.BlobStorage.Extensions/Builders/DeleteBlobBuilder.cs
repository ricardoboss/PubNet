using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class DeleteBlobBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? bucketName;
	private string? blobName;

	public DeleteBlobBuilder WithBucketName(string name)
	{
		this.bucketName = name;

		return this;
	}

	public DeleteBlobBuilder WithBlobName(string name)
	{
		blobName = name;

		return this;
	}

	public Task<bool> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		if (string.IsNullOrWhiteSpace(blobName))
			throw new MissingBlobNameException("The blob name was empty or whitespace but is required.");

		var args = new DeleteBlobArgs(bucketName, blobName);

		return storage.DeleteBlobAsync(args, cancellationToken);
	}
}
