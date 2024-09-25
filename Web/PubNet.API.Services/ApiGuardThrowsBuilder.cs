﻿using System.Security.Claims;
using PubNet.Web;
using PubNet.Web.Models;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.API.Services;

public class ApiGuardThrowsBuilder(IGuard guard, ClaimsPrincipal user) : IGuardThrowsBuilder
{
	private ScopesClaim Claim
	{
		get
		{
			var scopesClaimValue = user.FindFirstValue(JwtClaims.Scopes);

			return scopesClaimValue is null ? ScopesClaim.Empty : ScopesClaim.From(scopesClaimValue);
		}
	}

	public void Cannot(Scope scope, string? message = null)
	{
		if (guard.Allows(Claim, scope))
			return;

		throw new UnauthorizedAccessException(message ?? $"Missing claim for: {scope}");
	}

	public void CannotAny(IEnumerable<Scope> scopes, string? message = null)
	{
		var scopesArray = scopes.ToArray();
		if (scopesArray.Any(scope => guard.Allows(Claim, scope)))
			return;

		throw new UnauthorizedAccessException(message ?? $"Missing claim for any of: {string.Join(", ", scopesArray)}");
	}
}
