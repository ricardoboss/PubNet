using System.Net;
using System.Security.Authentication;

namespace PubNet.API.Middlewares;

public class ClientExceptionFormatterMiddleware(RequestDelegate next)
{
	public async Task Invoke(HttpContext context)
	{
		try
		{
			await next.Invoke(context);
		}
		catch (Exception e)
		{
			await Handle(context, e);
		}
	}

	private static async Task Handle(HttpContext context, Exception e)
	{
		if (e is InvalidCredentialException)
		{
			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			context.Response.Headers.WWWAuthenticate = new[]
			{
				$"Bearer realm=\"pubnet\", message=\"{e.Message}\"",
			};
		}
		else
		{
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		}

		await context.Response.WriteAsJsonAsync(
			new
			{
				e.Message,
				e.StackTrace,
			},
			options: null,
			contentType: "application/vnd.pub.v2+json"
		);
	}
}
