using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Abstractions;

public interface IBlobStorage
{
	string Name { get; }

	Task<bool> BucketExistsAsync(IBucketExistsArgs args, CancellationToken cancellationToken = default);

	Task<bool> CreateBucketAsync(ICreateBucketArgs args, CancellationToken cancellationToken = default);

	Task<bool> DeleteBucketAsync(IDeleteBucketArgs args, CancellationToken cancellationToken = default);

	Task<IBucketItem> GetBucketAsync(IGetBucketArgs args, CancellationToken cancellationToken = default);

	IAsyncEnumerable<IBucketItem> ListBucketsAsync(IListBucketsArgs args, CancellationToken cancellationToken = default);

	Task<bool> BlobExistsAsync(IBlobExistsArgs args, CancellationToken cancellationToken = default);

	Task<string> PutBlobAsync(IPutBlobArgs args, CancellationToken cancellationToken = default);

	Task<bool> DeleteBlobAsync(IDeleteBlobArgs args, CancellationToken cancellationToken = default);

	Task<IBlobItem> GetBlobAsync(IGetBlobArgs args, CancellationToken cancellationToken = default);

	IAsyncEnumerable<IBlobItem> ListBlobsAsync(IListBlobsArgs args, CancellationToken cancellationToken = default);
}
