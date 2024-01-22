using System.Diagnostics;
using System.Net;
using System.Security.Authentication;

namespace PubNet.API.Middlewares;

[DebuggerStepThrough]
public class ExceptionFormatterMiddleware(RequestDelegate next)
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
				error = new {
					code = e.GetType().Name,
					e.Message,
					StackTrace = e.StackTrace?.Split("\r\n").Select(x => x.Split("\n")).ToList(),
				},
			},
			options: null,
			contentType: "application/json"
		);
	}
}
