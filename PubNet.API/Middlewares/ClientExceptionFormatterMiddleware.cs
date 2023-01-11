﻿using System.Net;
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

        if (e is BearerTokenException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.Headers.WWWAuthenticate = new[]
            {
                $"Bearer realm=\"pub\", message=\"{e.Message}\""
            };
        }

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var exceptionClass = e.GetType().Name;
        var errorMessage = $"Error ({exceptionClass}): {e.Message}";

        await context.Response.WriteAsJsonAsync(
            new ErrorResponse(new(exceptionClass, errorMessage)),
            options: null,
            contentType: "application/vnd.pub.v2+json"
        );
    }

    private bool ShouldHandle(Exception e)
    {
        return true;
    }
}
