using Microsoft.Kiota.Abstractions.Authentication;

namespace PubNet.Frontend.Services;

public class AccessTokenProvider(AuthenticationService authenticationService) : IAccessTokenProvider
{
	public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var token = await authenticationService.GetTokenAsync(cancellationToken);

		return $"Bearer {token}";
	}

	public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator
	{
		AllowedHosts =
		[
			"https://localhost:7171",
		],
	};
}
