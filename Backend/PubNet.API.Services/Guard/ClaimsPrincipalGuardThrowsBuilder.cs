using System.Security.Claims;
using PubNet.API.Abstractions.Guard;
using PubNet.API.Services.Extensions;
using PubNet.Auth;
using PubNet.Auth.Models;

namespace PubNet.API.Services.Guard;

public class ClaimsPrincipalGuardThrowsBuilder(IGuard guard, ClaimsPrincipal user) : IGuardThrowsBuilder
{
	private ScopesClaim UserScopesClaim
	{
		get
		{
			var scopesClaimValue = user.FindFirstValue(JwtClaims.Scopes);

			return scopesClaimValue is null ? ScopesClaim.Empty : ScopesClaim.From(scopesClaimValue);
		}
	}

	private Role UserRoleClaim
	{
		get
		{
			var roleClaimValue = user.FindFirstValue(JwtClaims.Roles);

			return roleClaimValue?.ToRole() ?? Role.Unspecified;
		}
	}

	public void Cannot(Scope scope, string? message = null)
	{
		if (guard.Allows(UserScopesClaim, scope))
			return;

		throw new MissingScopeException(UserScopesClaim.ToScopes().ToList(), missingScope: scope, message);
	}

	public void CannotAny(IEnumerable<Scope> scopes, string? message = null)
	{
		var scopesArray = scopes.ToArray();
		if (scopesArray.Any(scope => guard.Allows(UserScopesClaim, scope)))
			return;

		throw new MissingScopeException(UserScopesClaim.ToScopes().ToList(), missingScopes: scopesArray, message);
	}

	public void DoesntHaveRole(Role role, string? message = null)
	{
		if (!guard.Allows(UserRoleClaim, toActAs: role))
			return;

		throw new InvalidRoleException(UserRoleClaim, role, message);
	}
}
