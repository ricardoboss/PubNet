using System.Diagnostics.CodeAnalysis;

namespace PubNet.API.Middlewares;

public class PubClientRewriterMiddleware(ILogger<PubClientRewriterMiddleware> logger) : IMiddleware
{
	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (!MatchesPubClientRequest(context, out var path))
			return next(context);

		// rewrite request from /api/packages/ to /Packages/Dart/
		context.Request.Path = path.Replace("/api/packages/", "/Packages/Dart/", StringComparison.OrdinalIgnoreCase);

		logger.LogTrace("Rewrote request path from {OldPath} to {NewPath}", path, context.Request.Path.Value);

		return next(context);
	}

	private static bool MatchesPubClientRequest(HttpContext context, [NotNullWhen(true)] out string? path)
	{
		path = context.Request.Path.Value;
		if (path is null)
			return false;

		path = context.Request.PathBase + path;
		if (!path.StartsWith("/api/packages/", StringComparison.OrdinalIgnoreCase))
			return false;

		var accept = context.Request.GetTypedHeaders().Accept;
		if (!accept.Any(x => x.MediaType.StartsWith("application/vnd.pub.v2+json", StringComparison.OrdinalIgnoreCase)))
			return false;

		return true;
	}
}
