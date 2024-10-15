using PubNet.API.Abstractions.Guard;
using PubNet.Auth.Models;

namespace PubNet.API.Services.Extensions;

public static class GuardThrowsBuilderExtensions
{
	public static void DoesntHave(this IGuardThrowsBuilder builder, Scope scope) => builder.Cannot(scope);
	public static void DoesntHaveAny(this IGuardThrowsBuilder builder, IEnumerable<Scope> scopes) => builder.CannotAny(scopes);
	public static void IsntA(this IGuardThrowsBuilder builder, Role role) => builder.HasntRole(role);
}
