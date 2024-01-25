using PubNet.Web.Models;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.Web;

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
