namespace PubNet.SDK.Exceptions;

/// <summary>
/// A setting value was rejected by the API's validation.
/// </summary>
public class InvalidSettingValueException(string message, Exception innerException)
	: PubNetSdkException(message, innerException);
