namespace PubNet.API.Abstractions.Packages.Dart;

public class DartPackageVersionOutdatedException : InvalidDartPackageException
{
	public DartPackageVersionOutdatedException(string message) : base(message)
	{
	}

	public DartPackageVersionOutdatedException(string message, Exception innerException) : base(message, innerException)
	{
	}

	public DartPackageVersionOutdatedException(string packageName, string packageVersion, string latestVersion) : base($"Package {packageName} version {packageVersion} is older than the latest version ({latestVersion})")
	{
	}
}
