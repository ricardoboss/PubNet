using PubNet.Web.Models;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.Web.Extensions;

public static class GuardExtensions
{
	public static IGuardThrowsBuilder ThrowIf(this IGuard guard, ScopesClaim claim) => new GuardThrowsBuilder(guard, claim);
}
