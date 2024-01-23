using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class ListBucketsBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? pattern;

	public ListBucketsBuilder WithPattern(string newPattern)
	{
		pattern = newPattern;

		return this;
	}

	public IAsyncEnumerable<IBucketItem> RunAsync(CancellationToken cancellationToken = default)
	{
		var args = new ListBucketsArgs(pattern);

		return storage.ListBucketsAsync(args, cancellationToken);
	}
}
