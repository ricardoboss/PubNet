using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class GetBucketBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? _bucketName;

	public GetBucketBuilder WithBucketName(string bucketName)
	{
		_bucketName = bucketName;

		return this;
	}

	public Task<IBucketItem> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(_bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		var args = new GetBucketArgs(_bucketName);

		return storage.GetBucketAsync(args, cancellationToken);
	}
}
