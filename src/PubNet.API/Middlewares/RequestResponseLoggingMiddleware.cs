using System.Text;

namespace PubNet.API.Middlewares;

public sealed class RequestResponseLoggingMiddleware(
	RequestDelegate next,
	ILogger<RequestResponseLoggingMiddleware> logger)
{
	private const int MaxLoggedBodyLength = 32 * 1024; // 32 KB

	public async Task Invoke(HttpContext context)
	{
		var requestInfo = await ReadRequestAsync(context);

		var originalResponseBody = context.Response.Body;

		await using var responseBuffer = new MemoryStream();
		context.Response.Body = responseBuffer;

		try
		{
			await next(context);

			var responseInfo = await ReadResponseAsync(context, responseBuffer);

			logger.LogInformation(
				"""
				HTTP {Method} {Path}
				Request:
				{Request}

				Response:
				Status: {StatusCode}
				{Response}
				""",
				context.Request.Method,
				context.Request.Path,
				requestInfo,
				context.Response.StatusCode,
				responseInfo);

			responseBuffer.Position = 0;
			await responseBuffer.CopyToAsync(originalResponseBody);
		}
		finally
		{
			context.Response.Body = originalResponseBody;
		}
	}

	private static async Task<string> ReadRequestAsync(HttpContext context)
	{
		var request = context.Request;

		request.EnableBuffering();

		if (!ShouldDumpBody(request.ContentType))
		{
			return $"Content-Type={request.ContentType ?? "(none)"}, Length={request.ContentLength ?? 0} bytes";
		}

		request.Body.Position = 0;

		using var reader = new StreamReader(
			request.Body,
			Encoding.UTF8,
			detectEncodingFromByteOrderMarks: false,
			leaveOpen: true);

		var body = await reader.ReadToEndAsync();

		request.Body.Position = 0;

		if (body.Length > MaxLoggedBodyLength)
		{
			body = body[..MaxLoggedBodyLength] + "... (truncated)";
		}

		return body;
	}

	private static async Task<string> ReadResponseAsync(
		HttpContext context,
		MemoryStream responseBuffer)
	{
		responseBuffer.Position = 0;

		if (!ShouldDumpBody(context.Response.ContentType))
		{
			return $"Content-Type={context.Response.ContentType ?? "(none)"}, Length={responseBuffer.Length} bytes";
		}

		using var reader = new StreamReader(
			responseBuffer,
			Encoding.UTF8,
			detectEncodingFromByteOrderMarks: false,
			leaveOpen: true);

		var body = await reader.ReadToEndAsync();

		if (body.Length > MaxLoggedBodyLength)
		{
			body = body[..MaxLoggedBodyLength] + "... (truncated)";
		}

		responseBuffer.Position = 0;

		return body;
	}

	private static bool ShouldDumpBody(string? contentType)
	{
		if (string.IsNullOrWhiteSpace(contentType))
			return false;

		contentType = contentType.ToLowerInvariant();

		return contentType.Contains("application/json")
			|| contentType.Contains("text/")
			|| contentType.Contains("application/xml")
			|| contentType.Contains("text/xml")
			|| contentType.Contains("application/problem+json");
	}
}
