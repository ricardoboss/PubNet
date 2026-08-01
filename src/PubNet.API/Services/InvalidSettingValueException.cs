namespace PubNet.API.Services;

public class InvalidSettingValueException(string key, string reason)
	: Exception($"The value for '{key}' is invalid: {reason}")
{
	public string Key { get; } = key;

	public string Reason { get; } = reason;
}
