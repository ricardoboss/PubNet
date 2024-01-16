namespace PubNet.BlobStorage.LocalFileBlobStorage;

public class BlobMetadataDeserializationFailureException : LocalFileBlobStorageException
{
	public string BucketName { get; }

	public string BlobName { get; }

	/// <inheritdoc />
	public BlobMetadataDeserializationFailureException(string message, string bucketName, string blobName) : base(message)
	{
		BucketName = bucketName;
		BlobName = blobName;
	}

	/// <inheritdoc />
	public BlobMetadataDeserializationFailureException(string message, string bucketName, string blobName, Exception innerException) : base(message, innerException)
	{
		BucketName = bucketName;
		BlobName = blobName;
	}
}
