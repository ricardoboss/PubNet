﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Rewrite;

namespace PubNet.API;

public class PubClientRewriteRule : IRule
{
	private const string PubContentType = "application/vnd.pub.v2+json";

	// public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	// {
	// 	if (!MatchesPubClientRequest(context, out var path))
	// 	{
	// 		await next(context);
	//
	// 		return;
	// 	}
	//
	// 	// rewrite request from /api/packages/ to /Packages/Dart/
	// 	context.Request.Path = path.Replace("/api/packages/", "/Packages/Dart/", StringComparison.OrdinalIgnoreCase);
	//
	// 	// logger.LogTrace("Rewrote request path from {OldPath} to {NewPath}", path, context.Request.Path.Value);
	//
	// 	await next(context);
	//
	// 	if (context.Response.HasStarted)
	// 		return;
	//
	// 	context.Response.Headers.ContentType = PubContentType;
	// }

	private static bool MatchesPubClientRequest(HttpContext context, [NotNullWhen(true)] out string? path)
	{
		path = context.Request.Path.Value;
		if (path is null)
			return false;

		path = context.Request.PathBase + path;
		if (!path.StartsWith("/api/packages/", StringComparison.OrdinalIgnoreCase))
			return false;

		var accept = context.Request.GetTypedHeaders().Accept;

		return accept.Any(x => x.MediaType.StartsWith(PubContentType, StringComparison.OrdinalIgnoreCase));
	}

	public void ApplyRule(RewriteContext context)
	{
		if (!MatchesPubClientRequest(context.HttpContext, out var path))
			return;

		// rewrite request from /api/packages/ to /Packages/Dart/
		context.HttpContext.Request.Path = path.Replace("/api/packages/", "/Packages/Dart/", StringComparison.OrdinalIgnoreCase);

		context.Result = RuleResult.ContinueRules;
	}
}
