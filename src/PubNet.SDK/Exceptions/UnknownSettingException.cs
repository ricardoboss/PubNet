namespace PubNet.SDK.Exceptions;

/// <summary>
/// A setting key was rejected because it is not configurable at runtime.
/// </summary>
public class UnknownSettingException(string message, Exception innerException)
	: PubNetSdkException(message, innerException);
