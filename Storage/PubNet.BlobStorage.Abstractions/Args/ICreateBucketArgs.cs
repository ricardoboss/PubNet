namespace PubNet.BlobStorage.Abstractions.Args;

public interface ICreateBucketArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }
}
