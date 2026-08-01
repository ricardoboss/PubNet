using PubNet.API.DTO;

namespace PubNet.Frontend.Services;

/// <summary>
/// Caches whether this instance still needs to be set up.
/// </summary>
public class OnboardingService(ApiClient apiClient, ILogger<OnboardingService> logger)
{
	private OnboardingStatusDto? _status;

	public async Task<OnboardingStatusDto> GetStatusAsync(CancellationToken cancellationToken = default)
	{
		if (_status is not null)
			return _status;

		try
		{
			_status = await apiClient.GetAsync<OnboardingStatusDto>("onboarding/status", cancellationToken);
		}
		catch (Exception e)
		{
			// an unreachable API is reported by the pages themselves; do not block navigation because of it
			logger.LogWarning(e, "Unable to determine the onboarding status");
		}

		return _status ??= new();
	}

	public async Task<bool> IsPendingAsync(CancellationToken cancellationToken = default)
	{
		return (await GetStatusAsync(cancellationToken)).Pending;
	}

	/// <summary>
	/// Drops the cached status, e.g. after onboarding has been completed.
	/// </summary>
	public void Invalidate()
	{
		_status = null;
	}
}
