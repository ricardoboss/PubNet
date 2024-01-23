using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class BlobExistsBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? bucketName;
	private string? blobName;

	public BlobExistsBuilder WithBucketName(string name)
	{
		bucketName = name;

		return this;
	}

	public BlobExistsBuilder WithBlobName(string name)
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

		var args = new BlobExistsArgs(bucketName, blobName);

		return storage.BlobExistsAsync(args, cancellationToken);
	}
}
