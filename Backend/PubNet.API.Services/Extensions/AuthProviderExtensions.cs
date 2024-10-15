using PubNet.API.Abstractions.Authentication;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.Extensions;

public static class AuthProviderExtensions
{
	/// <exception cref="UnauthorizedAccessException">Thrown when the <see cref="IAuthProvider"/> fails to provide an <see cref="Identity"/></exception>
	public static async Task<Identity> GetCurrentIdentityAsync(this IAuthProvider authProvider, CancellationToken cancellationToken = default)
	{
		var maybeIdentity = await authProvider.TryGetCurrentIdentityAsync(cancellationToken);
		if (maybeIdentity is null)
			throw new UnauthorizedAccessException("Unauthorized request");

		return maybeIdentity;
	}

	/// <exception cref="UnauthorizedAccessException">Thrown when the <see cref="IAuthProvider"/> fails to provide a <see cref="Token"/></exception>
	public static async Task<Token> GetCurrentTokenAsync(this IAuthProvider authProvider, CancellationToken cancellationToken = default)
	{
		var maybeToken = await authProvider.TryGetCurrentTokenAsync(cancellationToken);
		if (maybeToken is null)
			throw new UnauthorizedAccessException("Unauthorized request");

		return maybeToken;
	}
}
