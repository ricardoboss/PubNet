using System.Security.Authentication;
using System.Security.Claims;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services;

public class ApplicationRequestContext
{
	private Identity? _identity;

	private static InvalidCredentialException MissingAuthentication => new(
		"Missing authentication. Acquire a Bearer token at [POST /authentication/login] and send it in the 'Authenticate' header.");

	public async Task<Identity> RequireIdentityAsync(ClaimsPrincipal user, PubNetContext db,
		CancellationToken cancellationToken = default)
	{
		if (_identity is not null)
			return _identity;

		var idStr = user.FindFirstValue("id");
		if (idStr is null || !Guid.TryParse(idStr, out var id))
			throw MissingAuthentication;

		var identity = await db.Identities.FindAsync([id], cancellationToken);
		if (identity is null)
			throw MissingAuthentication;

		return _identity = identity;
	}
}
