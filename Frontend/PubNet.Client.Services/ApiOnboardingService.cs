using Microsoft.Kiota.Abstractions;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;

namespace PubNet.Client.Services;

public class ApiOnboardingService(PubNetApiClient client) : IOnboardingService
{
	public async Task<bool> IsSetupCompleteAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await client.Setup.Status.GetAsync(cancellationToken: cancellationToken);
			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			return result.SetupComplete ?? false;
		}
		catch (ApiException e)
		{
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}
}
