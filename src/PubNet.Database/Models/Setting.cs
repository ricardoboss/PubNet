using System.ComponentModel.DataAnnotations;

namespace PubNet.Database.Models;

/// <summary>
/// A single instance-level value which outlives a restart but does not belong in the configuration files:
/// the record that onboarding has been completed, and the settings an admin may change at runtime.
/// </summary>
public class Setting
{
	[Key]
	[MaxLength(256)]
	public string Key { get; set; } = string.Empty;

	public string? Value { get; set; }

	public DateTimeOffset UpdatedAtUtc { get; set; }
}
