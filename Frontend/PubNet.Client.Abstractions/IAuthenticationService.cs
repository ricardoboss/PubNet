using PubNet.Auth.Models;
using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface IAuthenticationService
{
	/// <param name="token">The token to use for authentication. If not provided, the token will be retrieved from the login token storage.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>The <see cref="AuthorDto"/> of the current user.</returns>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
	/// <exception cref="InvalidResponseException">Thrown when the response is invalid.</exception>
	Task<AuthorDto> GetSelfAsync(JsonWebToken? token = null, CancellationToken cancellationToken = default);
}
