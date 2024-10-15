namespace PubNet.BlobStorage.Abstractions.Args;

public interface IBlobExistsArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }

	string BlobName { get; }
}
