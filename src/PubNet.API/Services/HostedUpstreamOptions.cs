using Microsoft.Extensions.DependencyInjection;

namespace PubNet.API.Services;

public class HostedUpstreamOptions
{
	public const string ConfigKey = "HostedUpstream";

	public string BaseUrl { get; set; } = "https://pub.dev/api/";
}

public static class HostedUpstreamOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddHostedUpstreamOptions()
		{
			services.AddOptions<HostedUpstreamOptions>()
				.BindConfiguration(HostedUpstreamOptions.ConfigKey)
				.Validate(
					o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out var u) &&
					     (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps),
					"HostedUpstream:BaseUrl must be an absolute http or https URL")
				.ValidateOnStart();

			return services;
		}
	}
}
