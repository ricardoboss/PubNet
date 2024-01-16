namespace PubNet.BlobStorage.Extensions.Builders;

public class MissingBlobContentStreamException : ArgsBuilderException
{
	/// <inheritdoc />
	public MissingBlobContentStreamException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	public MissingBlobContentStreamException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
