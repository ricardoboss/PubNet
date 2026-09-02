using JetBrains.Annotations;

namespace PubNet.API.DTO.Settings;

/// <summary>
/// A configurable setting together with its currently effective value.
/// </summary>
[PublicAPI]
public class SettingDescriptorDto
{
	public string Key { get; init; } = null!;

	public string Group { get; init; } = null!;

	public string Label { get; init; } = null!;

	public string? Description { get; init; }

	public SettingKind Kind { get; init; }

	/// <summary>
	/// The currently effective value, or <c>null</c> for <see cref="SettingKind.Secret"/> settings.
	/// </summary>
	public string? Value { get; init; }
}
