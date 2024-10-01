using System.Net;
using Microsoft.Kiota.Abstractions;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;
using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Services;

public class ApiPersonalAccessTokenService(PubNetApiClient apiClient) : IPersonalAccessTokenService
{
	public async Task<IEnumerable<TokenDto>> GetAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var result =
				await apiClient.Authentication.PersonalAccessToken.GetAsync(cancellationToken: cancellationToken);

			if (result is not { Tokens: { } tokens })
				throw new InvalidResponseException("No response could be deserialized");

			return tokens;
		}
		catch (ApiException e) when (e.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException("Authentication is required for this request", e);
		}
		catch (ApiException e)
		{
			throw new InvalidResponseException("API returned an unexpected status code", e);
		}
	}

	public async Task<TokenDto> CreateAsync(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
