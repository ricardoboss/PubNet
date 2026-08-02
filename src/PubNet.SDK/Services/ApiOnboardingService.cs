using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class ApiOnboardingService(PubNetApiClient apiClient) : IOnboardingService
{
	public async Task<bool> IsPendingAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var status = await apiClient.Onboarding.Status.GetAsync(cancellationToken: cancellationToken);

			return status?.Pending ?? false;
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}

	public async Task<AuthorDto> CompleteAsync(RegisterRequestDto request,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var author = await apiClient.Onboarding.Complete.PostAsync(request, cancellationToken: cancellationToken);

			return author ?? throw new UnexpectedResponseException("Unable to deserialize onboarding response");
		}
		catch (OnboardingNotPendingErrorDto e)
		{
			throw new OnboardingNotPendingException(e);
		}
		catch (MissingRegistrationDataErrorDto e)
		{
			throw new MissingRegistrationDataException(e);
		}
		catch (UsernameAlreadyInUseErrorDto e)
		{
			throw new UsernameAlreadyRegisteredException(request.Username ?? "", e);
		}
		catch (EmailAlreadyInUseErrorDto e)
		{
			throw new EmailAlreadyRegisteredException(request.Email ?? "", e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}
}
