namespace PubNet.API.DTO.Settings;

/// <summary>
/// Describes how a setting is rendered and validated.
/// </summary>
public enum SettingKind
{
	Text = 0,

	Boolean = 1,

	Url = 2,

	/// <summary>
	/// Like <see cref="Text"/>, but never sent back to clients.
	/// </summary>
	Secret = 3,
}
