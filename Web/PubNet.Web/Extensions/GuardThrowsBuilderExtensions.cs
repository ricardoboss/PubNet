using PubNet.Web.Models;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.Web.Extensions;

public static class GuardThrowsBuilderExtensions
{
	public static void DoesntHave(this IGuardThrowsBuilder builder, Scope scope) => builder.Cannot(scope);
	public static void DoesntHaveAny(this IGuardThrowsBuilder builder, IEnumerable<Scope> scopes) => builder.CannotAny(scopes);
}
