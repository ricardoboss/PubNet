namespace PubNet.BlobStorage.LocalFileBlobStorage;

public class InvalidBlobNameException : LocalFileBlobStorageException
{
	/// <inheritdoc />
	public InvalidBlobNameException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	public InvalidBlobNameException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
