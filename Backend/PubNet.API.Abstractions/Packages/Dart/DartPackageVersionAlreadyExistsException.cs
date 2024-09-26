namespace PubNet.API.Abstractions.Packages.Dart;

public class DartPackageVersionAlreadyExistsException : InvalidDartPackageException
{
	public DartPackageVersionAlreadyExistsException(string message) : base(message)
	{
	}

	public DartPackageVersionAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
	{
	}

	public DartPackageVersionAlreadyExistsException(string packageName, string packageVersion) : base($"Package {packageName} version {packageVersion} already exists")
	{
	}
}
