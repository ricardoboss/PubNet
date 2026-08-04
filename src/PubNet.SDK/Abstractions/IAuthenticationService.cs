using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

/// <summary>
/// Provides authentication-related services like login or getting the authenticated author model.
/// </summary>
public interface IAuthenticationService
{
	/// <summary>
	/// Checks if registrations are enabled.
	/// </summary>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns><see langword="true"/> if registrations are enabled, <see langword="false"/> if not</returns>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<bool> GetRegistrationsEnabledAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Determines if the service is currently authenticated.
	/// </summary>
	/// <remarks>
	/// Fails closed: if the token store cannot be read, the answer is <see langword="false"/> rather than
	/// an exception, and the failure is logged. Cancellation still propagates.
	/// </remarks>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns><see langword="true"/> if the service is authenticated, <see langword="false"/> if not</returns>
	Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Tries to log in using the given credentials and returns a <see cref="JsonWebTokenResponseDto"/> if the login was
	/// successful.
	/// </summary>
	/// <param name="email">The e-mail address</param>
	/// <param name="password">The password</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A <see cref="JsonWebTokenResponseDto"/> containing a token to use for authentication</returns>
	/// <exception cref="InvalidLoginCredentialsException">If the given credentials are wrong</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<JsonWebTokenResponseDto> LoginAsync(string email, string password,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Asks the instance to send a password reset link to the given e-mail address.
	/// </summary>
	/// <param name="email">The e-mail address to send the reset link to</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <exception cref="EmailNotFoundException">If no account exists for the given <paramref name="email"/></exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sets a new password using a reset token obtained via <see cref="RequestPasswordResetAsync"/>.
	/// </summary>
	/// <param name="token">The reset token from the link that was e-mailed</param>
	/// <param name="password">The new password</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <exception cref="InvalidPasswordResetTokenException">If the token is invalid, expired or has already been used</exception>
	/// <exception cref="InvalidPasswordException">If the new password was rejected</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task ResetPasswordAsync(string token, string password, CancellationToken cancellationToken = default);

	/// <summary>
	/// Removes any authentication from the service.
	/// </summary>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A task representing the asynchronous request</returns>
	Task LogoutAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Registers a new author.
	/// </summary>
	/// <param name="email">The e-mail address to register</param>
	/// <param name="name">The display name for the author</param>
	/// <param name="password">The password for the new author</param>
	/// <param name="username">The authors username</param>
	/// <param name="website">The website of the author</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>An <see cref="AuthorDto"/> containing the registered author</returns>
	/// <exception cref="EmailAlreadyRegisteredException">If the given <paramref name="email"/> is already registered</exception>
	/// <exception cref="MissingRegistrationDataException">In case any required data is not set properly</exception>
	/// <exception cref="RegistrationsDisabledException">In case registrations are disabled; check using <see cref="GetRegistrationsEnabledAsync"/></exception>
	/// <exception cref="UsernameAlreadyRegisteredException">If the given <paramref name="username"/> is already registered</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<AuthorDto> RegisterAsync(string email, string name, string password, string username, string? website = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the currently authenticated author.
	/// </summary>
	/// <remarks>
	/// Automatically performs a logout (using <see cref="LogoutAsync"/>) if authentication fails.
	/// Uses <see cref="IsAuthenticatedAsync"/> to determine if the service is authenticated.
	/// </remarks>
	/// <param name="forceLoad">Whether to allow returning a cached instance. Passing <see langword="true"/> will return a new instance</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A <see cref="AuthorDto"/> representing the currently authenticated author</returns>
	/// <exception cref="AuthenticationRequiredException">If the service is not authenticated or the authentication is no longer valid</exception>
	/// <exception cref="PubNetSdkException">In case anything unexpected happens</exception>
	Task<AuthorDto> GetSelfAsync(bool forceLoad = false, CancellationToken cancellationToken = default);

	/// <summary>
	/// Drops the cached author returned by <see cref="GetSelfAsync"/>, so the next call loads it again.
	/// </summary>
	/// <remarks>
	/// Called automatically when an author is modified, since that author may be the authenticated one.
	/// </remarks>
	void InvalidateSelf();
}
