using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using PubNet.Client.Abstractions;

namespace PubNet.Client.Services;

public class LoginTokenAuthenticationProvider(ILoginTokenStorage loginTokenStorage, ILogger<LoginTokenAuthenticationProvider> logger) : IAuthenticationProvider
{
	public async Task AuthenticateRequestAsync(RequestInformation request,
		Dictionary<string, object>? additionalAuthenticationContext = null,
		CancellationToken cancellationToken = default)
	{
		if (!ShouldAuthenticate(request))
		{
			logger.LogTrace("Skipping authentication for request {Request}", request.URI);

			return;
		}

		logger.LogTrace("Authenticating request {Request}", request.URI);

		var token = await loginTokenStorage.GetTokenAsync(cancellationToken);
		if (token is null)
			throw new UnauthorizedAccessException($"Authentication is required for {request.URI}");

		request.Headers.Add("Authorization", $"Bearer {token}");
	}

	private readonly (Method method, string path)[] unauthenticatedEndpoints =
	[
		(Method.POST, "/Authentication/LoginToken"),
		(Method.GET, "/Authentication/RegistrationsOpen"),
		(Method.POST, "/Authentication/Account"),
	];

	private bool ShouldAuthenticate(RequestInformation request)
	{
		logger.LogTrace("Checking if request {Request} should be authenticated", request.URI);

		var path = request.URI.AbsolutePath;

		return !unauthenticatedEndpoints.Any(exemption =>
		{
			if (!path.EndsWith(exemption.path, StringComparison.OrdinalIgnoreCase))
				return false;

			return exemption.method == request.HttpMethod;
		});
	}
}
