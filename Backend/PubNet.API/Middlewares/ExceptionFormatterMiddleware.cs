using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using PubNet.API.Abstractions;
using PubNet.API.DTO;
using PubNet.API.DTO.Errors;
using PubNet.API.Services.Extensions;
using PubNet.API.Services.Guard;
using PubNet.Auth;

namespace PubNet.API.Middlewares;

public class ExceptionFormatterMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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
		object dto;
		JsonTypeInfo info;
		switch (e)
		{
			case OperationCanceledException or TaskCanceledException:
				context.Connection.RequestClose();

				return;
			case MissingScopeException missingScopeException:
				context.Response.StatusCode = PubNetHttpStatusCodes.Status460MissingScope;

				info = DtoSerializerContext.Default.MissingScopeErrorDto;
				dto = new MissingScopeErrorDto
				{
					Error = new()
					{
						Code = "missing_scope",
						Message = "One or more additional scopes is required to perform this action.",
					},
					GivenScopes = missingScopeException.AvailableScopes.Select(s => s.Value).ToArray(),
					RequiredScopes = missingScopeException.MissingScopes.Select(s => s.Value).ToArray(),
				};

				break;
			case InvalidRoleException invalidRoleException:
				context.Response.StatusCode = PubNetHttpStatusCodes.Status461InvalidRole;

				info = DtoSerializerContext.Default.InvalidRoleErrorDto;
				dto = new InvalidRoleErrorDto
				{
					Error = new()
					{
						Code = "invalid_role",
						Message = "A different role is required to perform this action.",
					},
					GivenRole = invalidRoleException.GivenRole.ToClaimValue(),
					RequiredRole = invalidRoleException.RequiredRole.ToClaimValue(),
				};

				break;
			case UnauthorizedAccessException:
				context.Response.StatusCode = StatusCodes.Status403Forbidden;

				info = DtoSerializerContext.Default.AuthErrorDto;
				dto = new AuthErrorDto
				{
					Error = new()
					{
						Code = "unauthorized",
						Message = e.Message,
					},
				};

				break;
			case InvalidCredentialException or AuthenticationException:
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.Response.Headers.WWWAuthenticate = new[]
				{
					$"Bearer realm=\"pubnet\", message=\"{e.Message}\"",
				};

				info = DtoSerializerContext.Default.AuthErrorDto;
				dto = new AuthErrorDto
				{
					Error = new()
					{
						Code = "authentication_failed",
						Message = e.Message,
					},
				};

				break;
			case ApiException apiException:
				context.Response.StatusCode = apiException.StatusCode;

				info = DtoSerializerContext.Default.GenericErrorDto;
				dto = new GenericErrorDto
				{
					Error = new()
					{
						Code = apiException.Code,
						Message = apiException.Message,
					},
#if DEBUG
					StackTrace = apiException.StackTrace?.Split("\r\n")
						.SelectMany(x => x.Split("\n"))
						.ToArray(),
#endif
				};

				break;
			default:
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				info = DtoSerializerContext.Default.InternalServerErrorDto;
				dto = new InternalServerErrorDto
				{
					Error = new()
					{
						Code = JsonNamingPolicy.SnakeCaseLower.ConvertName(e.GetType().Name),
						Message = e.Message,
					},
#if DEBUG
					StackTrace = e.StackTrace?.Split("\r\n")
						.SelectMany(x => x.Split("\n"))
						.ToArray(),
#endif
				};

				break;
		}

		await context.Response.WriteAsJsonAsync(dto, info, cancellationToken: context.RequestAborted);
	}
}
