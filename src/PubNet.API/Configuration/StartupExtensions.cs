using PubNet.API.Interfaces;

namespace PubNet.API.Configuration;

public static class StartupExtensions
{
	extension(WebApplication app)
	{
		/// <summary>
		/// Applies the settings stored in the database. Called after the migrations, because the settings table
		/// does not exist yet when the configuration is first built.
		/// </summary>
		public void ReloadSettings()
		{
			using var scope = app.Services.CreateScope();

			scope.ServiceProvider.GetRequiredService<ISettingsService>().ReloadConfiguration();
		}
	}
}
