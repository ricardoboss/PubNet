using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using PubNet.API.DTO;

namespace PubNet.API.Middlewares;

public class ExceptionFormatterMiddleware(RequestDelegate next)
{
	[DebuggerStepThrough]
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
		if (e is InvalidCredentialException or AuthenticationException or UnauthorizedAccessException)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			context.Response.Headers.WWWAuthenticate = new[]
			{
				$"Bearer realm=\"pubnet\", message=\"{e.Message}\"",
			};
		}
		else
		{
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
		}

		await context.Response.WriteAsJsonAsync(
			new GenericErrorDto
			{
				Error = new()
				{
					Code = e.GetType().Name,
					Message = e.Message,
					StackTrace = e.StackTrace?.Split("\r\n")
						.SelectMany(x => x.Split("\n"))
						.ToArray(),
				},
			},
			options: DtoGenerationContext.Default.Options,
			cancellationToken: context.RequestAborted
		);
	}
}
