using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class CreateBucketBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? _bucketName;

	public CreateBucketBuilder WithBucketName(string bucketName)
	{
		_bucketName = bucketName;

		return this;
	}

	public Task<bool> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(_bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		var args = new CreateBucketArgs(_bucketName);

		return storage.CreateBucketAsync(args, cancellationToken);
	}
}
