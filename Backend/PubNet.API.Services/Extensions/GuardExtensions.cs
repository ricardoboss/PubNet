using System.Security.Claims;
using PubNet.API.Abstractions.Guard;
using PubNet.API.Services.Guard;
using PubNet.Auth.Models;

namespace PubNet.API.Services.Extensions;

public static class GuardExtensions
{
	public static IGuardThrowsBuilder ThrowIf(this IGuard guard, ScopesClaim claim) => new ScopesClaimGuardThrowsBuilder(guard, claim);

	public static IGuardThrowsBuilder ThrowIf(this IGuard guard, ClaimsPrincipal claim) => new ClaimsPrincipalGuardThrowsBuilder(guard, claim);
}
