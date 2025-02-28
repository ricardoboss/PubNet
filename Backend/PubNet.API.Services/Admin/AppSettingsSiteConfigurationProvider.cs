using Microsoft.Extensions.Configuration;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Admin;

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
