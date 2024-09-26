using PubNet.API.Abstractions.Guard;
using PubNet.Auth.Models;

namespace PubNet.API.Services.Guard;

public class ScopesClaimGuardThrowsBuilder(IGuard guard, ScopesClaim claim) : IGuardThrowsBuilder
{
	public void Cannot(Scope scope, string? message = null)
	{
		if (guard.Allows(claim, scope))
			return;

		throw new UnauthorizedAccessException(message ?? $"Missing claim for: {scope}");
	}

	public void CannotAny(IEnumerable<Scope> scopes, string? message = null)
	{
		var scopesArray = scopes.ToArray();
		if (scopesArray.Any(scope => guard.Allows(claim, scope)))
			return;

		throw new UnauthorizedAccessException(message ?? $"Missing claim for any of: {string.Join(", ", scopesArray)}");
	}
}
