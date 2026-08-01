using PubNet.API.DTO;
using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface IAuthorRegistrationService
{
	/// <summary>
	/// Validates the request, creates an author with the given role and saves it.
	/// </summary>
	/// <remarks>
	/// Whether an account may be created at all is up to the caller: registration is gated on the instance
	/// settings, onboarding on there being no admin yet.
	/// </remarks>
	/// <exception cref="Services.AuthorRegistrationException">
	/// The request is incomplete, or the username or e-mail address is already taken.
	/// </exception>
	Task<Author> RegisterAsync(RegisterRequest request, Role role, CancellationToken cancellationToken = default);
}
