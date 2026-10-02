using Microsoft.EntityFrameworkCore;
using PubNet.API.Configuration;
using PubNet.API.DTO.Settings;
using PubNet.API.Interfaces;
using PubNet.Database;

namespace PubNet.API.Services;

/// <summary>
/// Reads and writes the instance settings declared in the <see cref="SettingsRegistry"/>.
/// </summary>
public class SettingsService(
	PubNetContext db,
	IConfiguration configuration,
	SettingsRegistry registry,
	ILogger<SettingsService> logger
) : ISettingsService
{
	/// <inheritdoc />
	public IReadOnlyList<SettingDescriptorDto> GetAll()
	{
		return registry.Descriptors
			.OrderBy(d => d.Group, StringComparer.OrdinalIgnoreCase)
			.ThenBy(d => d.Label, StringComparer.OrdinalIgnoreCase)
			.Select(d => new SettingDescriptorDto
			{
				Key = d.Key,
				Group = d.Group,
				Label = d.Label,
				Description = d.Description,
				Kind = d.Kind,
				// secrets are write-only; there is no reason to hand them back out
				Value = d.Kind == SettingKind.Secret ? null : configuration[d.Key],
			})
			.ToList();
	}

	/// <inheritdoc />
	public async Task ApplyAsync(IReadOnlyDictionary<string, string?> values,
		CancellationToken cancellationToken = default)
	{
		if (values.Count == 0)
			return;

		// validate everything before touching the database so a bad value cannot half-apply a change set
		var validated = new Dictionary<string, string?>(StringComparer.Ordinal);
		foreach (var (key, value) in values)
		{
			if (!registry.TryGet(key, out var descriptor))
				throw new UnknownSettingException(key);

			Validate(descriptor, value);

			// use the canonical spelling from the registry, which is what the configuration provider looks for
			validated[descriptor.Key] = value;
		}

		var keys = validated.Keys.ToList();
		var existing = await db.Settings
			.Where(s => keys.Contains(s.Key))
			.ToDictionaryAsync(s => s.Key, cancellationToken);

		var removed = new List<string>();
		foreach (var (key, value) in validated)
		{
			existing.TryGetValue(key, out var setting);

			if (value is null)
			{
				// no value means "fall back to the configuration files"
				if (setting is not null)
					db.Settings.Remove(setting);

				removed.Add(key);

				continue;
			}

			if (setting is null)
			{
				db.Settings.Add(new() { Key = key, Value = value, UpdatedAtUtc = DateTimeOffset.UtcNow });

				continue;
			}

			setting.Value = value;
			setting.UpdatedAtUtc = DateTimeOffset.UtcNow;
		}

		await db.SaveChangesAsync(cancellationToken);

		// only the keys, never the values: some settings are secrets
		logger.LogInformation("Applied instance settings: set {SettingKeys}, reset {ResetSettingKeys}",
			keys.Except(removed).ToList(), removed);

		// inside a transaction the caller decides when the changes become visible, because the configuration
		// provider reads through its own connection and would not see uncommitted rows
		if (db.Database.CurrentTransaction is null)
			ReloadConfiguration();
	}

	/// <inheritdoc />
	public void ReloadConfiguration()
	{
		if (configuration is IConfigurationRoot root)
			root.Reload();
		else
			logger.LogWarning("Configuration is not reloadable; setting changes apply after a restart");
	}

	private static void Validate(SettingDescriptor descriptor, string? value)
	{
		if (value is null)
			return;

		switch (descriptor.Kind)
		{
			case SettingKind.Boolean when !bool.TryParse(value, out _):
				throw new InvalidSettingValueException(descriptor.Key, "expected 'true' or 'false'");
			case SettingKind.Url when !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
			                          (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps):
				throw new InvalidSettingValueException(descriptor.Key, "expected an absolute http or https URL");
			case SettingKind.Text:
			case SettingKind.Secret:
			default:
				return;
		}
	}
}
