using System.Text;
using Microsoft.Extensions.Primitives;
using PubNet.API.Contexts;
using PubNet.API.Extensions;
using PubNet.API.Services;

namespace PubNet.API.Middlewares;

public class ApplicationRequestContextEnricherMiddleware
{
    private readonly RequestDelegate _next;
    private readonly PubNetContext _db;

    public ApplicationRequestContextEnricherMiddleware(RequestDelegate next, PubNetContext db)
    {
        _next = next;
        _db = db;
    }

    public async Task Invoke(HttpContext context, ApplicationRequestContext requestContext)
    {
        if (context.Request.Headers.Accept.Count > 0)
        {
            EnrichAcceptContext(context.Request.Headers.Accept, requestContext);
        }

        if (context.Request.Headers.Authorization.Count > 0)
        {
            EnrichAuthContext(context.Request.Headers.Authorization.First()!, requestContext);
        }

        await _next.Invoke(context);
    }

    private void EnrichAuthContext(string authorization, ApplicationRequestContext requestContext)
    {
        if (!authorization.StartsWith("Bearer "))
        {
            return;
        }

        // base64 encoded token
        var bearer = authorization["Bearer ".Length..];

        // concatenated login data (username:token)
        var base64decoded = Encoding.UTF8.GetString(Convert.FromBase64String(bearer));

        var (username, token, _) = base64decoded.Split(':', 2);

        var author = _db.Authors.FirstOrDefault(a => a.Email == username);
        var authorToken = author?.Tokens.FirstOrDefault(t => t.Value == token);
        if (authorToken == null || !authorToken.IsValid())
        {
            return;
        }

        requestContext.Author = author;
        requestContext.AuthorToken = authorToken;
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
