using System.Diagnostics;
using Microsoft.Extensions.Primitives;
using PubNet.API.DTO;
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

        // if (httpContext.Request.Headers.Authorization.Count > 0)
        // {
        //     EnrichAuthContext(httpContext.Request.Headers.Authorization.First()!, requestContext);
        // }

        await _next.Invoke(httpContext);
    }

    // private void EnrichAuthContext(string authorization, ApplicationRequestContext requestContext)
    // {
    //     const string bearerPrefix = "Bearer ";
    //     if (!authorization.StartsWith(bearerPrefix))
    //         throw new BearerTokenException("Unsupported authorization type");
    //
    //     var token = authorization[bearerPrefix.Length..];
    //     var result = _bearerTokenManager.Verify(token, out var authorToken);
    //     if (result == BearerTokenManager.VerifyResult.InvalidFormat)
    //         throw new BearerTokenException("Invalid Bearer token format");
    //
    //     if (result is BearerTokenManager.VerifyResult.UnknownAuthor or BearerTokenManager.VerifyResult.UnknownToken)
    //         throw new BearerTokenException("Token not found. Get a new token at [POST /authors/{username}/tokens] or register an account at [POST /author]");
    //
    //     if (result == BearerTokenManager.VerifyResult.ExpiredToken)
    //         throw new BearerTokenException("Token expired. Get a new token at [POST /authors/{username}/tokens]");
    //
    //     if (result != BearerTokenManager.VerifyResult.Ok)
    //         throw new BearerTokenException("Unknown error verifying Bearer token");
    //
    //     Debug.Assert(authorToken is not null, "authorToken is not null");
    //
    //     requestContext.AuthorToken = authorToken;
    // }

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
