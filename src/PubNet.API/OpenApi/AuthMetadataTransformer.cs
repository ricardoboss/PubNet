using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using PubNet.API.DTO.Authentication.Errors;

namespace PubNet.API.OpenApi;

public class AuthMetadataTransformer : IOpenApiOperationTransformer
{
	public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		if (context.Description.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
			return;

		var firstRelevantAttribute =
			GetOrderedAttributes(actionDescriptor.MethodInfo, typeof(AuthorizeAttribute),
				typeof(AllowAnonymousAttribute)).FirstOrDefault();

		switch (firstRelevantAttribute)
		{
			case AuthorizeAttribute authorizeAttribute:
				await AddUnauthenticatedResponseAsync(operation, context, cancellationToken);
				AddSecurityScheme(operation, authorizeAttribute);
				break;
			case AllowAnonymousAttribute:
				operation.Responses?.Remove("401");
				operation.Security?.Clear();
				break;
		}
	}

	private static void AddSecurityScheme(OpenApiOperation operation, AuthorizeAttribute authorizeAttribute)
	{
		var requiredScheme = authorizeAttribute.AuthenticationSchemes ?? JwtBearerDefaults.AuthenticationScheme;

		operation.Security ??= [];
		operation.Security.Add(
			new()
			{
				{
					new(requiredScheme),
					[]
				},
			}
		);
	}

	private static async Task AddUnauthenticatedResponseAsync(OpenApiOperation operation,
		OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{
		var unauthenticatedResponse = new OpenApiResponse
		{
			Description = "Unauthenticated",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = await context.GetOrCreateSchemaAsync(typeof(MissingAuthenticationErrorDto),
						cancellationToken: cancellationToken),
				},
			},
		};

		operation.Responses ??= [];
		_ = operation.Responses["401"] = unauthenticatedResponse;
	}

	private static IEnumerable<Attribute> GetOrderedAttributes(MethodInfo methodInfo, params Type[] attributeTypes)
	{
		return methodInfo
			.GetCustomAttributes(true)
			.Cast<Attribute>()
			.Concat(methodInfo.DeclaringType?.GetCustomAttributes(true).Cast<Attribute>() ?? [])
			.Where(a => attributeTypes.Any(t => t.IsAssignableFrom(a.GetType())));
	}
}
