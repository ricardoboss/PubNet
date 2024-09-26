using Microsoft.Kiota.Abstractions;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;
using PubNet.Client.ApiClient.Generated.Models;
using PubNet.Auth.Models;

namespace PubNet.Client.Services;

public class ApiLoginService(PubNetApiClient apiClient) : ILoginService
{
	public async Task<LoginSuccessResult> LoginAsync(CreateLoginTokenDto request, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Authentication.LoginToken.PostAsync(request, cancellationToken: cancellationToken);
			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			if (result.Token is null)
				throw new InvalidResponseException("The response did not contain a token");

			var jwt = JsonWebToken.From(result.Token);

			return new(jwt);
		}
		catch (GenericErrorDto e) when (e.ResponseStatusCode == 401)
		{
			throw new UnauthorizedAccessException(e.Error?.Message ?? "The given credentials are incorrect");
		}
		catch (ApiException e)
		{
			throw new InvalidResponseException("API returned an unexpected status code", e);
		}
	}
}
