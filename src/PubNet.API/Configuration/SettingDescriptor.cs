using PubNet.API.DTO.Settings;

namespace PubNet.API.Configuration;

/// <summary>
/// Declares a configuration key as an instance setting: stored in the database, layered on top of the
/// configuration files and writable at runtime by an admin.
/// </summary>
/// <remarks>
/// Only declared keys are ever read from or written to the database, which keeps deployment-level
/// configuration (connection strings, JWT keys, allowed origins) out of reach of the admin backend.
/// </remarks>
public sealed record SettingDescriptor
{
	/// <summary>
	/// The configuration key, using <c>:</c> as the section separator (e.g. <c>HostedUpstream:BaseUrl</c>).
	/// </summary>
	public required string Key { get; init; }

	/// <summary>
	/// The section this setting is displayed under.
	/// </summary>
	public required string Group { get; init; }

	public required string Label { get; init; }

	public required SettingKind Kind { get; init; }

	public string? Description { get; init; }
}
