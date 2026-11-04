using PubNet.API.Configuration;
using PubNet.API.DTO.Settings;

namespace PubNet.API.Services;

public class HostedUpstreamOptions
{
	public const string ConfigKey = "HostedUpstream";

	public const string BaseUrlKey = $"{ConfigKey}:{nameof(BaseUrl)}";

	public string BaseUrl { get; set; } = "https://pub.dev/api/";

	public static IEnumerable<SettingDescriptor> Descriptors =>
	[
		new()
		{
			Key = BaseUrlKey,
			Group = "Packages",
			Label = "Hosted upstream URL",
			Description =
				"API base URL of the repository to fall back to for packages not hosted here, e.g. https://pub.dev/api/.",
			Kind = SettingKind.Url,
		},
	];
}
