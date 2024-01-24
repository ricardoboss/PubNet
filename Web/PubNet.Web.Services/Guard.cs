using PubNet.Web.Abstractions;
using PubNet.Web.Abstractions.Models;
using PubNet.Web.Abstractions.Services;

namespace PubNet.Web.Services;

public class Guard : IGuard
{
	public bool Allows(ScopesClaim claim, Scope scope)
	{
		if (claim.Value is null)
			return false;

		return claim.Value
			.Split(JwtClaims.ScopeSeparator)
			.Select(Scope.From)
			.Any(s => s.EqualsOrIsParentOf(scope));
	}

	public bool Denies(ScopesClaim claim, Scope scope)
	{
		return !Allows(claim, scope);
	}
}
