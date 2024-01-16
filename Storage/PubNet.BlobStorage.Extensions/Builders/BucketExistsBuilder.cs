using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class BucketExistsBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? _bucketName;

	public BucketExistsBuilder WithBucketName(string bucketName)
	{
		_bucketName = bucketName;

		return this;
	}

	public Task<bool> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(_bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		var args = new BucketExistsArgs(_bucketName);

		return storage.BucketExistsAsync(args, cancellationToken);
	}
}
