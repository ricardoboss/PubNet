using PubNet.API.Abstractions.Guard;
using PubNet.API.Attributes;
using PubNet.API.Services.Extensions;

namespace PubNet.API.Middlewares;

public class RoleGuardMiddleware(IGuard guard) : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var endpoint = context.GetEndpoint();
		if (endpoint is not null)
		{
			var requiredScopes = endpoint.Metadata.GetOrderedMetadata<RequireRoleAttribute>();

			foreach (var scopeAttribute in requiredScopes)
			{
				guard.ThrowIf(context.User).DoesntHaveRole(scopeAttribute.Role,
					$"This endpoint requires the role '{scopeAttribute.Role}'.");
			}
		}

		await next(context);
	}
}
