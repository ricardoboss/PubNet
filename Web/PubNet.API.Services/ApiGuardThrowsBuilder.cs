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
			var scopesClaimValue = user.FindFirstValue(JwtClaims.Scope);
			if (scopesClaimValue is null)
				throw new UnauthorizedAccessException("Missing scopes claim. Are you authenticated?");

			return ScopesClaim.From(scopesClaimValue);
		}
	}

	public void Cannot(Scope scope, string? message = null)
	{
		if (guard.Allows(Claim, scope))
			return;

		throw new UnauthorizedAccessException(message ?? $"Missing claim for: {scope}");
	}
}