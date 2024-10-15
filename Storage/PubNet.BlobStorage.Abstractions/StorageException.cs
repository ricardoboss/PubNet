namespace PubNet.BlobStorage.Abstractions;

public abstract class StorageException : Exception
{
	protected StorageException(string message) : base(message)
	{
	}

	protected StorageException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
