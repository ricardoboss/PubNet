using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class CreateBucketBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? bucketName;

	public CreateBucketBuilder WithBucketName(string name)
	{
		bucketName = name;

		return this;
	}

	public Task<bool> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		var args = new CreateBucketArgs(bucketName);

		return storage.CreateBucketAsync(args, cancellationToken);
	}
}
