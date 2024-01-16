namespace PubNet.BlobStorage.Abstractions.Args;

public interface IGetBucketArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }
}
