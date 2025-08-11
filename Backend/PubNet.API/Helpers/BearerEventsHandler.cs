using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using PubNet.API.DTO;
using PubNet.API.DTO.Errors;

namespace PubNet.API.Helpers;

internal sealed class BearerEventsHandler : JwtBearerEvents
{
	public override Task Forbidden(ForbiddenContext context)
	{
		return SendUnauthorized(context);
	}

	public override async Task Challenge(JwtBearerChallengeContext context)
	{
		// Suppress the default response on 401 challenge
		context.HandleResponse();

		var header = BuildWwwAuthenticateHeader(context);
		context.Response.Headers.Append(HeaderNames.WWWAuthenticate, header);

		await SendUnauthenticated(context);
	}

	private static async Task SendUnauthorized(ForbiddenContext context)
	{
		var dto = new AuthErrorDto
		{
			Error = new()
			{
				Code = "unauthorized",
				Message = "You are not authorized to perform this action.",
			},
		};

		context.Response.StatusCode = StatusCodes.Status403Forbidden;

		await context.Response.WriteAsJsonAsync(dto, DtoGenerationContext.Default.AuthErrorDto);
	}

	private const string FallbackUnauthenticatedError = "unauthenticated";

	private const string FallbackUnauthenticatedMessage =
		"Authentication is required for this request, but your token is missing or invalid.";

	private static async Task SendUnauthenticated(JwtBearerChallengeContext context)
	{
		var dto = new AuthErrorDto
		{
			Error = new()
			{
				Code = context.Error ?? FallbackUnauthenticatedError,
				Message = context.ErrorDescription ?? FallbackUnauthenticatedMessage,
			},
		};

		context.Response.StatusCode = StatusCodes.Status401Unauthorized;

		await context.Response.WriteAsJsonAsync(dto, DtoGenerationContext.Default.AuthErrorDto);
	}

	/// From: https://github.com/dotnet/aspnetcore/blob/d12868dd7c10ff0433c16b06d3b59d03c40d987a/src/Security/Authentication/JwtBearer/src/JwtBearerHandler.cs#L226
	private static string BuildWwwAuthenticateHeader(JwtBearerChallengeContext eventContext)
	{
		// https://tools.ietf.org/html/rfc6750#section-3.1
		// WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
		var builder = new StringBuilder(eventContext.Options.Challenge);

		builder.Append(" error=\"");
		builder.Append(!string.IsNullOrEmpty(eventContext.Error) ? eventContext.Error : FallbackUnauthenticatedError);
		builder.Append('"');

		builder.Append(", error_description=\"");
		builder.Append(!string.IsNullOrEmpty(eventContext.ErrorDescription)
			? eventContext.ErrorDescription
			: FallbackUnauthenticatedMessage);
		builder.Append('\"');

		if (!string.IsNullOrEmpty(eventContext.ErrorUri))
		{
			if (!string.IsNullOrEmpty(eventContext.Error) ||
				!string.IsNullOrEmpty(eventContext.ErrorDescription))
			{
				builder.Append(',');
			}

			builder.Append(" error_uri=\"");
			builder.Append(eventContext.ErrorUri);
			builder.Append('\"');
		}

		return builder.ToString();
	}
}
