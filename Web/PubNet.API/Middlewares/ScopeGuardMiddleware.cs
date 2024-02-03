using PubNet.API.Attributes;
using PubNet.API.Services.Extensions;
using PubNet.Web.Extensions;
using PubNet.Web.ServiceInterfaces;

namespace PubNet.API.Middlewares;

public class ScopeGuardMiddleware(IGuard guard) : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var endpoint = context.GetEndpoint();
		if (endpoint is not null)
		{
			var requiredScopes = endpoint.Metadata.GetOrderedMetadata<RequireScopeAttribute>();
			var anyOfRequiredScopes = endpoint.Metadata.GetOrderedMetadata<RequireAnyScopeAttribute>();

			foreach (var scopeAttribute in requiredScopes)
			{
				guard.ThrowIf(context.User).DoesntHave(scopeAttribute.Scope);
			}

			foreach (var scopeAttribute in anyOfRequiredScopes)
			{
				guard.ThrowIf(context.User).DoesntHaveAny(scopeAttribute.Scopes);
			}
		}

		await next(context);
	}
}
