namespace PubNet.API.Configuration;

public static class DatabaseConfigurationExtensions
{
	extension(IConfigurationBuilder builder)
	{
		public IConfigurationBuilder AddDatabaseConfiguration(SettingsRegistry registry, string? connectionString,
			ILoggerFactory loggerFactory)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				loggerFactory.CreateLogger<DatabaseConfigurationProvider>().LogWarning(
					"No 'PubNet' connection string configured; instance settings will not be read from the database");

				return builder;
			}

			return builder.Add(new DatabaseConfigurationSource(registry, connectionString, loggerFactory));
		}
	}
}
