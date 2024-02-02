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
			var scopeAttrs = endpoint.Metadata.GetOrderedMetadata<RequireScopeAttribute>();

			foreach (var scopeAttribute in scopeAttrs)
			{
				guard.ThrowIf(context.User).DoesntHave(scopeAttribute.Scope);
			}
		}

		await next(context);
	}
}
