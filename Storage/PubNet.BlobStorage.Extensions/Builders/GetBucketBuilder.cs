using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class GetBucketBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? bucketName;

	public GetBucketBuilder WithBucketName(string name)
	{
		bucketName = name;

		return this;
	}

	public Task<IBucketItem> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		var args = new GetBucketArgs(bucketName);

		return storage.GetBucketAsync(args, cancellationToken);
	}
}
