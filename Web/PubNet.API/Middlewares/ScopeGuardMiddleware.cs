using PubNet.API.Attributes;
using PubNet.API.Services.Extensions;
using PubNet.Web.Abstractions.Services;
using PubNet.Web.Services.Extensions;

namespace PubNet.API.Middlewares;

public class ScopeGuardMiddleware(IGuard guard) : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var endpoint = context.GetEndpoint();
		if (endpoint is not null)
		{
			var scopes = endpoint.Metadata
				.GetOrderedMetadata<GuardAttribute>()
				.Select(x => x.Require);

			foreach (var scope in scopes)
			{
				guard.ThrowIf(context.User).DoesntHave(scope);
			}
		}

		await next(context);
	}
}
