using Microsoft.EntityFrameworkCore;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Configuration;

/// <summary>
/// Reads instance settings from the <see cref="Setting"/> table and exposes them as configuration values.
/// </summary>
/// <remarks>
/// Registered as the last configuration source, so database values win over the files and the environment.
/// </remarks>
public sealed class DatabaseConfigurationProvider(
	SettingsRegistry registry,
	string connectionString,
	ILogger<DatabaseConfigurationProvider> logger
) : ConfigurationProvider
{
	private bool _loadedOnce;

	/// <inheritdoc />
	public override void Load()
	{
		var optionsBuilder = new DbContextOptionsBuilder<PubNetContext>()
			.UseNpgsql(connectionString);

		try
		{
			using var db = new PubNetContext(optionsBuilder.Options);

			var settings = db.Settings
				.AsNoTracking()
				.ToList()
				.Where(s => registry.Contains(s.Key));

			Data = settings.ToDictionary(s => s.Key, s => s.Value, StringComparer.OrdinalIgnoreCase);
			_loadedOnce = true;

			logger.LogInformation("Loaded {SettingCount} instance setting(s) from the database: {SettingKeys}",
				Data.Count, Data.Keys);
		}
		catch (Exception e)
		{
			if (_loadedOnce)
			{
				// a failed reload must not quietly revert to the file values, which could re-open
				// registrations or point the upstream somewhere else
				logger.LogError(e, "Unable to reload instance settings from the database, keeping the current values");

				return;
			}

			// expected on a fresh installation, where the configuration is built before the migrations run
			logger.LogInformation(e, "Unable to read instance settings from the database, using file configuration " +
				"only until the migrations have run");

			Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
		}
	}
}
