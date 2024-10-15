using PubNet.BlobStorage.Abstractions;

namespace PubNet.BlobStorage.LocalFileBlobStorage;

public class DirectoryBucketItem(DirectoryInfo directoryInfo) : IBucketItem
{
	/// <inheritdoc />
	public string BucketName => directoryInfo.Name;
}
