using PubNet.Web.Abstractions.Models;
using PubNet.Web.Abstractions.Services;

namespace PubNet.Web.Services.Extensions;

public static class GuardThrowsBuilderExtensions
{
	public static void DoesntHave(this IGuardThrowsBuilder builder, Scope scope) => builder.Cannot(scope);
}
