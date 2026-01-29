using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Services;

namespace PubNet.SDK.Extensions;

public static class PubNetServiceBuilderExtensions
{
	extension(IPubNetServiceBuilder builder)
	{
		public IPubNetServiceBuilder AddCaching()
		{
			var services = builder.Services;

			services.TryDecorate<IAnalysisService, CachingAnalysisService>();
			services.TryDecorate<IAuthorService, CachingAuthorService>();
			services.TryDecorate<IPackagesService, CachingPackagesService>();

			return builder;
		}
	}

	extension(IPubNetApiServiceBuilder builder)
	{
		public IPubNetApiServiceBuilder AddConcurrentRequestPrevention()
		{
			builder.Services.TryDecorate<IRequestAdapter, ConcurrentRequestBlockingRequestAdapter>();

			return builder;
		}
	}
}
