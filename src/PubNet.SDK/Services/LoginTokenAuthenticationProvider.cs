using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using PubNet.SDK.Abstractions;

namespace PubNet.SDK.Services;

/// <remarks>
/// The token is attached to every request that has one available. Endpoints that do not require
/// authentication declare so in the OpenAPI document and are annotated <c>[AllowAnonymous]</c> on the
/// server, so the header is simply never looked at - there is deliberately no allow list of
/// unauthenticated endpoints to keep in sync here.
/// </remarks>
internal sealed class LoginTokenAuthenticationProvider(ILoginTokenStorage loginTokenStorage, ILogger<LoginTokenAuthenticationProvider> logger) : IAuthenticationProvider
{
	public async Task AuthenticateRequestAsync(RequestInformation request,
		Dictionary<string, object>? additionalAuthenticationContext = null,
		CancellationToken cancellationToken = default)
	{
		var token = await loginTokenStorage.GetTokenAsync(cancellationToken);
		if (token is null)
		{
			logger.LogTrace("No token available for request {Request}", request.URI);

			return; // let service layer handle 401 rejections from the API
		}

		logger.LogTrace("Authenticating request {Request}", request.URI);

		request.Headers.Add("Authorization", $"Bearer {token}");
	}
}
