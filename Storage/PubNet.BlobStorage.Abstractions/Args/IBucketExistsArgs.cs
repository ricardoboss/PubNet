namespace PubNet.BlobStorage.Abstractions.Args;

public interface IBucketExistsArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }
}
