using PubNet.BlobStorage.Abstractions;

namespace PubNet.BlobStorage.Extensions.Builders;

public abstract class ArgsBuilderException : StorageException
{
	/// <inheritdoc />
	protected ArgsBuilderException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	protected ArgsBuilderException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
