namespace PubNet.API.Abstractions;

public interface ISiteConfigurationProvider
{
	Task<ISiteConfiguration> GetAsync(CancellationToken cancellationToken = default);
}
