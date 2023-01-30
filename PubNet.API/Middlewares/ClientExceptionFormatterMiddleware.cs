using System.Net;
using System.Security.Authentication;
using PubNet.API.DTO;

namespace PubNet.API.Middlewares;

public class ClientExceptionFormatterMiddleware
{
    private readonly RequestDelegate _next;

    public ClientExceptionFormatterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception e)
        {
            await Handle(context, e);
        }
    }

    private async Task Handle(HttpContext context, Exception e)
    {
        if (!ShouldHandle(e))
        {
            await _next.Invoke(context);

            return;
        }

        if (e is BearerTokenException or InvalidCredentialException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.Headers.WWWAuthenticate = new[]
            {
                $"Bearer realm=\"pub\", message=\"{e.Message}\"",
            };
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        await context.Response.WriteAsJsonAsync(
            ErrorResponse.FromException(e),
            options: null,
            contentType: "application/vnd.pub.v2+json"
        );
    }

    private bool ShouldHandle(Exception e)
    {
        return true;
    }
}
