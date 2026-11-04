namespace PubNet.API.Configuration;

public sealed class DatabaseConfigurationSource(
	SettingsRegistry registry,
	string connectionString,
	ILoggerFactory loggerFactory
) : IConfigurationSource
{
	/// <inheritdoc />
	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new DatabaseConfigurationProvider(registry, connectionString,
			loggerFactory.CreateLogger<DatabaseConfigurationProvider>());
	}
}
