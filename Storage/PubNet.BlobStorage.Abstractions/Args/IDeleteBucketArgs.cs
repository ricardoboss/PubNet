namespace PubNet.BlobStorage.Abstractions.Args;

public interface IDeleteBucketArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }
}
