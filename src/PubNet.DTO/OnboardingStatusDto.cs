using JetBrains.Annotations;

namespace PubNet.API.DTO;

/// <summary>
/// Tells clients whether this instance still needs to be set up.
/// </summary>
[PublicAPI]
public class OnboardingStatusDto
{
	/// <summary>
	/// Whether onboarding has not been completed yet. While <c>true</c>, registrations are closed and
	/// <c>POST /onboarding/complete</c> accepts an unauthenticated request creating the first admin.
	/// </summary>
	public bool Pending { get; init; }
}
