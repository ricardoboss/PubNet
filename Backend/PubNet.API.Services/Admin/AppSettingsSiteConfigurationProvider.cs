using Microsoft.Extensions.Configuration;
using PubNet.API.Abstractions;

namespace PubNet.API.Services;

public class AppSettingsSiteConfigurationProvider(IConfiguration configuration) : ISiteConfigurationProvider
{
	public Task<ISiteConfiguration> GetAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var config = new DelegateSiteConfiguration(configuration);

		return Task.FromResult<ISiteConfiguration>(config);
	}
}
