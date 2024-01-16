namespace PubNet.BlobStorage.Extensions.Builders;

public class MissingBlobNameException : ArgsBuilderException
{
	/// <inheritdoc />
	public MissingBlobNameException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	public MissingBlobNameException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
