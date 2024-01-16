namespace PubNet.BlobStorage.Abstractions.Args;

public interface IListBlobsArgs : IBlobStorageCommandArgs
{
	string BucketName { get; }

	string? Pattern { get; }

	string? ContentType { get; }
}
