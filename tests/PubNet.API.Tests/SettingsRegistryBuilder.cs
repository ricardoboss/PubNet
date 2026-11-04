using PubNet.API.Configuration;
using PubNet.API.DTO.Settings;
using PubNet.API.Services;

namespace PubNet.API.Tests;

internal sealed class SettingsRegistryBuilder
{
	public const string TextKey = "Test:Text";

	public const string SecretKey = "Test:Secret";

	private readonly List<SettingDescriptor> _descriptors = [.. RegistrationOptions.Descriptors];

	public SettingsRegistryBuilder WithTextSetting()
	{
		return With(new()
		{
			Key = TextKey,
			Group = "Test",
			Label = "Text",
			Kind = SettingKind.Text,
		});
	}

	public SettingsRegistryBuilder WithSecretSetting()
	{
		return With(new()
		{
			Key = SecretKey,
			Group = "Test",
			Label = "Secret",
			Kind = SettingKind.Secret,
		});
	}

	public SettingsRegistryBuilder With(SettingDescriptor descriptor)
	{
		_descriptors.Add(descriptor);

		return this;
	}

	public SettingsRegistry Build()
	{
		return new SettingsRegistry().Add(_descriptors);
	}
}
