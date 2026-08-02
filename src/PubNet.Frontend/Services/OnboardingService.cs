using PubNet.SDK.Abstractions;

namespace PubNet.Frontend.Services;

/// <summary>
/// Caches whether this instance still needs to be set up.
/// </summary>
public class OnboardingService(IOnboardingService onboarding, ILogger<OnboardingService> logger)
{
	private bool? _pending;

	public async Task<bool> IsPendingAsync(CancellationToken cancellationToken = default)
	{
		if (_pending is { } cached)
			return cached;

		try
		{
			_pending = await onboarding.IsPendingAsync(cancellationToken);
		}
		catch (Exception e)
		{
			// an unreachable API is reported by the pages themselves; do not block navigation because of it
			logger.LogWarning(e, "Unable to determine the onboarding status");
		}

		return _pending ??= false;
	}

	/// <summary>
	/// Drops the cached status, e.g. after onboarding has been completed.
	/// </summary>
	public void Invalidate()
	{
		_pending = null;
	}
}
