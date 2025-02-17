﻿using PubNet.API.Abstractions.Guard;
using PubNet.Auth;
using PubNet.Auth.Models;

namespace PubNet.API.Services.Guard;

public class Guard : IGuard
{
	public bool Allows(ScopesClaim claim, Scope targetScope)
	{
		return claim.Value
			.Split(JwtClaims.ScopeSeparator)
			.Where(s => !string.IsNullOrWhiteSpace(s))
			.Select(Scope.From)
			.Any(s => s.EqualsOrIsParentOf(targetScope));
	}

	public bool Allows(Role claimedRole, Role toActAs)
	{
		return claimedRole >= toActAs;
	}
}
