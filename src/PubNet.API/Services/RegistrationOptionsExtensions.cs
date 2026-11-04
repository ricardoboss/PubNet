using Microsoft.Extensions.DependencyInjection;

namespace PubNet.API.Services;

public static class RegistrationOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddRegistrationOptions(IConfiguration configuration)
		{
			// bound against the configuration root because 'OpenRegistration' is a top-level key
			services.AddOptions<RegistrationOptions>().Bind(configuration);

			return services;
		}
	}
}
