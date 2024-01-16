using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Args;

namespace PubNet.BlobStorage.Extensions.Builders;

public class ListBucketsBuilder(IBlobStorage storage) : IArgsBuilder
{
	private string? _pattern;

	public ListBucketsBuilder WithPattern(string pattern)
	{
		_pattern = pattern;

		return this;
	}

	public IAsyncEnumerable<IBucketItem> RunAsync(CancellationToken cancellationToken = default)
	{
		var args = new ListBucketsArgs(_pattern);

		return storage.ListBucketsAsync(args, cancellationToken);
	}
}
