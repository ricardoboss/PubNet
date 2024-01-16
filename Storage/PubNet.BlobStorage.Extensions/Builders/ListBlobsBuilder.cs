using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class ListBlobsBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? _bucketName;
	private string? _pattern;
	private string? _contentType;

	public ListBlobsBuilder WithBucketName(string bucketName)
	{
		_bucketName = bucketName;

		return this;
	}

	public ListBlobsBuilder WithPattern(string pattern)
	{
		_pattern = pattern;

		return this;
	}

	public ListBlobsBuilder WithContentType(string contentType)
	{
		_contentType = contentType;

		return this;
	}

	public IAsyncEnumerable<IBlobItem> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(_bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		var args = new ListBlobsArgs(_bucketName, _pattern, _contentType);

		return storage.ListBlobsAsync(args, cancellationToken);
	}
}
