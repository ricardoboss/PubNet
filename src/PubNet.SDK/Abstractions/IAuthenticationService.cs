using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

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
	/// Removes any authentication from the service.
	/// </summary>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns>A task representing the asynchronous request</returns>
	Task LogoutAsync(CancellationToken cancellationToken = default);

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
	/// Determines if the currently authenticated author has the given <paramref name="username"/>.
	/// </summary>
	/// <param name="username">The username to check against</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
	/// <returns><see langword="true"/> if the <paramref name="username"/> matches that of the currently authenticated user (case-sensitive), <see langword="false"/> if not</returns>
	Task<bool> IsSelfAsync(string username, CancellationToken cancellationToken = default);
}
