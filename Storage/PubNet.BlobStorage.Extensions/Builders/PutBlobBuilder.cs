using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class PutBlobBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? _bucketName;
	private string? _blobName;
	private string? _contentType;
	private Stream? _contentStream;

	public PutBlobBuilder WithBucketName(string bucketName)
	{
		_bucketName = bucketName;

		return this;
	}

	public PutBlobBuilder WithBlobName(string blobName)
	{
		_blobName = blobName;

		return this;
	}

	public PutBlobBuilder WithContentType(string contentType)
	{
		_contentType = contentType;

		return this;
	}

	public PutBlobBuilder WithContentStream(Stream contentStream)
	{
		_contentStream = contentStream;

		return this;
	}

	public Task<string> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(_bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		if (string.IsNullOrWhiteSpace(_blobName))
			throw new MissingBlobNameException("The blob name was empty or whitespace but is required.");

		if (string.IsNullOrWhiteSpace(_contentType))
			throw new MissingBlobContentTypeException("The content type was empty or whitespace but is required.");

		if (_contentStream is null)
			throw new MissingBlobContentStreamException("The content stream was null but is required.");

		var args = new PutBlobArgs(_bucketName, _blobName, _contentType, _contentStream);

		return storage.PutBlobAsync(args, cancellationToken);
	}
}
