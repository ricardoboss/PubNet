using PubNet.Web.Abstractions.Models;
using PubNet.Web.Abstractions.Services;

namespace PubNet.Web.Services;

public class GuardThrowsBuilder(IGuard guard, ScopesClaim claim) : IGuardThrowsBuilder
{
	public void Cannot(Scope scope, string? message = null)
	{
		if (guard.Allows(claim, scope))
			return;

		throw new UnauthorizedAccessException(message ?? $"Missing claim for: {scope}");
	}
}
