namespace PubNet.API.Abstractions.Packages.Dart;

public class InvalidDartPackageException : Exception
{
	public InvalidDartPackageException(string message) : base(message)
	{
	}

	public InvalidDartPackageException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
