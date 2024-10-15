using PubNet.BlobStorage.Abstractions;

namespace PubNet.BlobStorage.LocalFileBlobStorage;

public abstract class LocalFileBlobStorageException : StorageException
{
	/// <inheritdoc />
	protected LocalFileBlobStorageException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	protected LocalFileBlobStorageException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
