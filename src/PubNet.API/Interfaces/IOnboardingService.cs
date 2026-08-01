using PubNet.API.DTO;
using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface IOnboardingService
{
	/// <summary>
	/// Whether this instance still has to be set up.
	/// </summary>
	/// <remarks>
	/// Onboarding is pending until it has been completed explicitly. Losing every admin account does
	/// <em>not</em> re-open onboarding, otherwise deleting the last account would hand the instance to
	/// whoever asks for it first.
	/// </remarks>
	Task<bool> IsPendingAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Creates the first admin account and marks onboarding as completed.
	/// </summary>
	/// <exception cref="Services.OnboardingNotPendingException">Onboarding has already been completed.</exception>
	/// <exception cref="Services.AuthorRegistrationException">The account could not be created.</exception>
	Task<Author> CompleteAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
