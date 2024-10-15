using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class ListBlobsBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? bucketName;
	private string? pattern;
	private string? contentType;

	public ListBlobsBuilder WithBucketName(string name)
	{
		bucketName = name;

		return this;
	}

	public ListBlobsBuilder WithPattern(string newPattern)
	{
		pattern = newPattern;

		return this;
	}

	public ListBlobsBuilder WithContentType(string newContentType)
	{
		contentType = newContentType;

		return this;
	}

	public IAsyncEnumerable<IBlobItem> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		var args = new ListBlobsArgs(bucketName, pattern, contentType);

		return storage.ListBlobsAsync(args, cancellationToken);
	}
}
