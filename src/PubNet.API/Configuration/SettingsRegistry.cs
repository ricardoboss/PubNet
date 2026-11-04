using System.Diagnostics.CodeAnalysis;

namespace PubNet.API.Configuration;

/// <summary>
/// The set of configuration keys which may be overridden at runtime. Built in <c>Program.cs</c> before the
/// host, because the database configuration source needs it.
/// </summary>
public sealed class SettingsRegistry
{
	private readonly Dictionary<string, SettingDescriptor> _descriptors = new(StringComparer.OrdinalIgnoreCase);

	public SettingsRegistry Add(IEnumerable<SettingDescriptor> descriptors)
	{
		foreach (var descriptor in descriptors)
		{
			if (!_descriptors.TryAdd(descriptor.Key, descriptor))
				throw new InvalidOperationException($"A setting with the key '{descriptor.Key}' is already registered");
		}

		return this;
	}

	public IReadOnlyCollection<SettingDescriptor> Descriptors => _descriptors.Values;

	public bool Contains(string key)
	{
		return _descriptors.ContainsKey(key);
	}

	public bool TryGet(string key, [NotNullWhen(true)] out SettingDescriptor? descriptor)
	{
		return _descriptors.TryGetValue(key, out descriptor);
	}
}
