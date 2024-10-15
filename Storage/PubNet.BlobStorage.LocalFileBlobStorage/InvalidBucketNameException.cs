namespace PubNet.BlobStorage.LocalFileBlobStorage;

public class InvalidBucketNameException : LocalFileBlobStorageException
{
	/// <inheritdoc />
	public InvalidBucketNameException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	public InvalidBucketNameException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
