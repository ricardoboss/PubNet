namespace PubNet.BlobStorage.Extensions.Builders;

public class MissingBucketNameException : ArgsBuilderException
{
	/// <inheritdoc />
	public MissingBucketNameException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	public MissingBucketNameException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
