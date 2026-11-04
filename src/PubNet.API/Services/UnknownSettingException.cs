namespace PubNet.API.Services;

public class UnknownSettingException(string key) : Exception($"'{key}' is not a configurable setting")
{
	public string Key { get; } = key;
}
