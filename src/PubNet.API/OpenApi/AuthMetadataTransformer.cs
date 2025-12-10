using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using PubNet.API.DTO.Errors;

namespace PubNet.API.OpenApi;

public class AuthMetadataTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		HandleAuthRequirementAndResponse(operation, context);

		return Task.CompletedTask;
	}

	private static void HandleAuthRequirementAndResponse(OpenApiOperation operation, OpenApiOperationTransformerContext context)
	{
		if (context.Description.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
			return;

		var firstRelevantAttribute = GetOrderedAttributes(actionDescriptor.MethodInfo, typeof(AuthorizeAttribute), typeof(AllowAnonymousAttribute)).FirstOrDefault();

		// in case of null or 'Allow Anonymous' attribute, we don't add any security requirements
		if (firstRelevantAttribute is not AuthorizeAttribute authorizeAttribute)
			return;

		AddUnauthenticatedResponse(operation);

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

	private static void AddUnauthenticatedResponse(OpenApiOperation operation)
	{
		var unauthenticatedResponse = new OpenApiResponse
		{
			Description = "Unauthenticated",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = new OpenApiSchemaReference(nameof(AuthErrorDto)),
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
