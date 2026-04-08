namespace PubNet.SDK.Exceptions;

public class PackageVersionNotFoundException(string packageName, string packageVersion, Exception innerException)
	: PubNetSdkException($"Package version not found: {packageName} {packageVersion}", innerException);
