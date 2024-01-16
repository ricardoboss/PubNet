using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class BlobExistsBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? _bucketName;
	private string? _blobName;

	public BlobExistsBuilder WithBucketName(string bucketName)
	{
		_bucketName = bucketName;

		return this;
	}

	public BlobExistsBuilder WithBlobName(string blobName)
	{
		_blobName = blobName;

		return this;
	}

	public Task<bool> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(_bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		if (string.IsNullOrWhiteSpace(_blobName))
			throw new MissingBlobNameException("The blob name was empty or whitespace but is required.");

		var args = new BlobExistsArgs(_bucketName, _blobName);

		return storage.BlobExistsAsync(args, cancellationToken);
	}
}
