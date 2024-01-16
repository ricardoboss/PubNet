namespace PubNet.BlobStorage.Extensions.Builders;

public class MissingBlobContentTypeException : ArgsBuilderException
{
	/// <inheritdoc />
	public MissingBlobContentTypeException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	public MissingBlobContentTypeException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
