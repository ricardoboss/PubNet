namespace PubNet.API.Abstractions.Admin;

public interface ISiteConfigurationProvider
{
	Task<ISiteConfiguration> GetAsync(CancellationToken cancellationToken = default);
}
