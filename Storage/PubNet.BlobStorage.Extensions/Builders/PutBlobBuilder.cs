using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class PutBlobBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? bucketName;
	private string? blobName;
	private string? contentType;
	private Stream? contentStream;

	public PutBlobBuilder WithBucketName(string name)
	{
		bucketName = name;

		return this;
	}

	public PutBlobBuilder WithBlobName(string name)
	{
		blobName = name;

		return this;
	}

	public PutBlobBuilder WithContentType(string newContentType)
	{
		contentType = newContentType;

		return this;
	}

	public PutBlobBuilder WithContentStream(Stream newContentStream)
	{
		contentStream = newContentStream;

		return this;
	}

	public Task<string> RunAsync(CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(bucketName))
			throw new MissingBucketNameException("The bucket name was empty or whitespace but is required.");

		if (string.IsNullOrWhiteSpace(blobName))
			throw new MissingBlobNameException("The blob name was empty or whitespace but is required.");

		if (string.IsNullOrWhiteSpace(contentType))
			throw new MissingBlobContentTypeException("The content type was empty or whitespace but is required.");

		if (contentStream is null)
			throw new MissingBlobContentStreamException("The content stream was null but is required.");

		var args = new PutBlobArgs(bucketName, blobName, contentType, contentStream);

		return storage.PutBlobAsync(args, cancellationToken);
	}
}
