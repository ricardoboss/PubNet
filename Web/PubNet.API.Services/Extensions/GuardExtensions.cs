using System.Security.Claims;
using PubNet.Web;
using PubNet.Web.Models;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.API.Services.Extensions;

public static class GuardExtensions
{
	public static bool Allows(this IGuard guard, ClaimsPrincipal principal, Scope scope)
	{
		var scopesString = principal.FindFirstValue(JwtClaims.Scope);

		return scopesString != null && guard.Allows(ScopesClaim.From(scopesString), scope);
	}

	public static IGuardThrowsBuilder ThrowIf(this IGuard guard, ClaimsPrincipal claim) => new ApiGuardThrowsBuilder(guard, claim);
}
