namespace PubNet.API.Middlewares;

public class PubClientRewriterMiddleware(ILogger<PubClientRewriterMiddleware> logger) : IMiddleware
{
	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (!MatchesPubClientRequest(context))
			return next(context);

		var path = context.Request.Path.Value;
		if (path is null)
			return next(context);

		if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
			return next(context);

		// rewrite request from /api/ to /Packages/Dart/
		context.Request.Path = path.Replace("/api/", "/Packages/Dart/", StringComparison.OrdinalIgnoreCase);

		logger.LogTrace("Rewrote request path from {OldPath} to {NewPath}", path, context.Request.Path.Value);

		return next(context);
	}

	private static bool MatchesPubClientRequest(HttpContext context)
	{
		var accept = context.Request.GetTypedHeaders().Accept;

		return accept.Any(x => x.MediaType.StartsWith("application/vnd.pub.v2+json", StringComparison.OrdinalIgnoreCase));
	}
}
