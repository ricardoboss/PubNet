using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

/// <summary>
/// First-time setup of an instance.
/// </summary>
public interface IOnboardingService
{
	/// <summary>
	/// Whether this instance still needs to be set up.
	/// </summary>
	Task<bool> IsPendingAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Creates the first administrator, closing onboarding for good.
	/// </summary>
	/// <exception cref="Exceptions.OnboardingNotPendingException">The instance has already been set up.</exception>
	/// <exception cref="Exceptions.RegisterException">The request is incomplete or the account already exists.</exception>
	Task<AuthorDto> CompleteAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
}
