using System.Security.Claims;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.API.Services.Extensions;

public static class GuardExtensions
{
	public static IGuardThrowsBuilder ThrowIf(this IGuard guard, ClaimsPrincipal claim) => new ApiGuardThrowsBuilder(guard, claim);
}
