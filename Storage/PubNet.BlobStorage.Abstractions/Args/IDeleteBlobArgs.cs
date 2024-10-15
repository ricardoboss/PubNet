namespace PubNet.BlobStorage.Abstractions.Args;

public interface IDeleteBlobArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }

	string BlobName { get; }
}
