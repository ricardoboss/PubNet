using Microsoft.Extensions.DependencyInjection;

namespace PubNet.API.Services;

/// <summary>
/// Where the frontend lives. Links in mails point here; it must never be derived from request headers, since those
/// are attacker controlled.
/// </summary>
public class FrontendOptions
{
	public const string ConfigKey = "Frontend";

	public string BaseUrl { get; set; } = "";
}

public static class FrontendOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddFrontendOptions()
		{
			services.AddOptions<FrontendOptions>()
				.BindConfiguration(FrontendOptions.ConfigKey)
				.Validate(
					o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out var u) &&
					     (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps),
					"Frontend:BaseUrl must be an absolute http or https URL")
				.ValidateOnStart();

			return services;
		}
	}
}
