using System.Security.Authentication;
using PubNet.API.Controllers;
using PubNet.API.DTO.Errors;

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
			context.Response.StatusCode = PubNetStatusCodes.Status401Unauthenticated;
			context.Response.Headers.WWWAuthenticate = new[]
			{
				$"Bearer realm=\"pub\", message=\"{e.Message}\"",
			};
		}
		else
		{
			context.Response.StatusCode = PubNetStatusCodes.Status500InternalServerError;
		}

		await context.Response.WriteAsJsonAsync(
			new InternalServerErrorDto
			{
				Error = new()
				{
					Code = e.GetType().Name,
					Message = e.Message,
				},
				StackTrace = e.StackTrace is { } st ? [.. st.Split("\r\n").SelectMany(l => l.Split('\n'))] : null,
			},
			options: null,
			contentType: "application/vnd.pub.v2+json"
		);
	}
}
