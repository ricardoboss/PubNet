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
		(Method.POST, "/Authentication/Account"),
		(Method.GET, "/Setup/Status"),
		(Method.POST, "/Setup/RootAdmin"),
	];

	private bool ShouldAuthenticate(RequestInformation request)
	{
		if (request.Headers.ContainsKey("Authorization"))
		{
			logger.LogTrace("Request to {Request} already has an Authorization header", request.URI);

			return false;
		}

		var path = request.URI.AbsolutePath;
		var hasExemption = unauthenticatedEndpoints.Any(exemption =>
		{
			if (!path.EndsWith(exemption.path, StringComparison.OrdinalIgnoreCase))
				return false;

			return exemption.method == request.HttpMethod;
		});

		return !hasExemption;
	}
}
