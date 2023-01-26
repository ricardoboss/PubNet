using Microsoft.Extensions.Primitives;
using PubNet.API.Extensions;
using PubNet.API.Services;

namespace PubNet.API.Middlewares;

public class ApplicationRequestContextEnricherMiddleware
{
    private readonly RequestDelegate _next;

    public ApplicationRequestContextEnricherMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, ApplicationRequestContext requestContext)
    {
        if (httpContext.Request.Headers.Accept.Count > 0)
        {
            EnrichAcceptContext(httpContext.Request.Headers.Accept, requestContext);
        }

        await _next.Invoke(httpContext);
    }

    private static void EnrichAcceptContext(StringValues acceptValues, ApplicationRequestContext context)
    {
        const string vendorPrefix = "application/vnd.pub.v";
        foreach (var accept in acceptValues.Where(accept => accept is not null && accept.StartsWith(vendorPrefix)).Cast<string>())
        {
            var (version, format, _) = accept[vendorPrefix.Length..].Split('+', 2);

            context.AcceptedApiVersions.Add(version);
            context.AcceptedResponseFormats.Add(format);
        }
    }
}
