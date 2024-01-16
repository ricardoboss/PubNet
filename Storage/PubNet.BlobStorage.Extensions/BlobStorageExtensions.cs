using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Extensions.Builders;

namespace PubNet.BlobStorage.Extensions;

public static class BlobStorageExtensions
{
	public static BucketExistsBuilder BucketExists(this IBlobStorage storage) => new(storage);

	public static CreateBucketBuilder CreateBucket(this IBlobStorage storage) => new(storage);

	public static DeleteBucketBuilder DeleteBucket(this IBlobStorage storage) => new(storage);

	public static GetBucketBuilder GetBucket(this IBlobStorage storage) => new(storage);

	public static ListBucketsBuilder ListBuckets(this IBlobStorage storage) => new(storage);

	public static BlobExistsBuilder BlobExists(this IBlobStorage storage) => new(storage);

	public static PutBlobBuilder PutBlob(this IBlobStorage storage) => new(storage);

	public static DeleteBlobBuilder DeleteBlob(this IBlobStorage storage) => new(storage);

	public static GetBlobBuilder GetBlob(this IBlobStorage storage) => new(storage);

	public static ListBlobsBuilder ListBlobs(this IBlobStorage storage) => new(storage);
}
