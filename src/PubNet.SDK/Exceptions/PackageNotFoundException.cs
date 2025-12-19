namespace PubNet.SDK.Exceptions;

public class PackageNotFoundException(string packageName, Exception innerException)
	: PubNetSdkException($"Could not find package '{packageName}'", innerException);
