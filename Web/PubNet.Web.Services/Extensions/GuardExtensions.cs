using PubNet.Web.Abstractions.Models;
using PubNet.Web.Abstractions.Services;

namespace PubNet.Web.Services.Extensions;

public static class GuardExtensions
{
	public static IGuardThrowsBuilder ThrowIf(this IGuard guard, ScopesClaim claim) => new GuardThrowsBuilder(guard, claim);
}
