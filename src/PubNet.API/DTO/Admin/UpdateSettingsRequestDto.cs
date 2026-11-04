using JetBrains.Annotations;

namespace PubNet.API.DTO.Admin;

[PublicAPI]
public class UpdateSettingsRequestDto
{
	/// <summary>
	/// The settings to change, keyed by setting key. A <c>null</c> value removes the override, falling back
	/// to the value from the configuration files.
	/// </summary>
	public Dictionary<string, string?> Settings { get; set; } = [];
}
