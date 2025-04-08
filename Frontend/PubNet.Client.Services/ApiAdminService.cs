using Microsoft.Kiota.Abstractions;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;
using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Services;

public class ApiAdminService(PubNetApiClient client) : IAdminService
{
	public async Task<IdentityDto> CreateAccountAsync(CreateAccountDto data, CancellationToken cancellationToken = default)
	{
		try
		{
			var identity = await client.Admin.Identity.PostAsync(data, cancellationToken: cancellationToken);

			if (identity is null)
				throw new InvalidResponseException("Response could not be deserialized.");

			return identity;
		}
		catch (ApiException e)
		{
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}

	public async Task<IEnumerable<IdentityDto>> GetAccountsAsync(CancellationToken cancellationToken = default)
	{
		try {
			var result = await client.Admin.Identities.GetAsync(cancellationToken: cancellationToken);
			if (result is null)
				throw new InvalidResponseException("Response could not be deserialized.");

			return result;
		}
		catch (ApiException e)
		{
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}

	public async Task DeleteIdentityAsync(Guid identityId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
