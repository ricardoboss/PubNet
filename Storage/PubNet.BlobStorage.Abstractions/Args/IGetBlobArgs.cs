namespace PubNet.BlobStorage.Abstractions.Args;

public interface IGetBlobArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }

	string BlobName { get; }
}
