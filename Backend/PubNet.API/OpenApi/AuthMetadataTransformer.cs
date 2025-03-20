using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using PubNet.API.Attributes;
using PubNet.API.DTO.Errors;
using PubNet.Auth;

namespace PubNet.API.OpenApi;

public class AuthMetadataTransformer : IOpenApiOperationTransformer, IOpenApiDocumentTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		HandleAuthRequirementAndResponse(operation, context);

		HandleScopeRequirementAndResponse(operation, context);

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

		AddInvalidRoleResponse(operation);

		var requiredScheme = authorizeAttribute.AuthenticationSchemes ?? JwtBearerDefaults.AuthenticationScheme;

		operation.Security ??= [];
		operation.Security.Add(
			new()
			{
				{
					new()
					{
						Reference = new()
						{
							Type = ReferenceType.SecurityScheme,
							Id = requiredScheme,
						},
					},
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
					Schema = new()
					{
						Reference = new()
						{
							Type = ReferenceType.Schema,
							Id = nameof(AuthErrorDto),
						},
					},
				},
			},
		};

		operation.Responses ??= [];
		_ = operation.Responses.TryAdd("401", unauthenticatedResponse);
	}

	private static void HandleScopeRequirementAndResponse(OpenApiOperation operation, OpenApiOperationTransformerContext context)
	{
		if (context.Description.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
			return;

		var requiredScopes = GetOrderedAttributes(actionDescriptor.MethodInfo, typeof(RequireScopeAttribute))
			.Select(a => ((RequireScopeAttribute) a).Scope)
			.Select(s => s.Value)
			.ToArray();

		var anyOfRequiredScopes = GetOrderedAttributes(actionDescriptor.MethodInfo, typeof(RequireAnyScopeAttribute))
			.Select(a => ((RequireAnyScopeAttribute) a).Scopes)
			.Select(s => s.Select(ss => ss.Value).ToArray())
			.ToArray();

		if (requiredScopes.Length == 0 && anyOfRequiredScopes.Length == 0)
			return;

		AddMissingRequiredScopeResponse(operation);

		if (requiredScopes.Length > 0)
		{
			var requiredScopesExtension = new RequiredScopesExtension(requiredScopes);

			operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();
			operation.Extensions.Add("x-required-scopes", requiredScopesExtension);
		}

		if (anyOfRequiredScopes.Length > 0)
		{
			var anyOfRequiredScopesExtension = new AnyOfRequiredScopesExtension(anyOfRequiredScopes);

			operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();
			operation.Extensions.Add("x-any-of-required-scopes", anyOfRequiredScopesExtension);
		}
	}

	private static void AddMissingRequiredScopeResponse(OpenApiOperation operation)
	{
		var requiredScopesResponse = new OpenApiResponse
		{
			Description = "Missing required scope",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = new()
					{
						Reference = new()
						{
							Type = ReferenceType.Schema,
							Id = nameof(MissingScopeErrorDto),
						},
					},
				},
			},
		};

		operation.Responses ??= [];
		_ = operation.Responses.TryAdd(PubNetHttpStatusCodes.Status460MissingScope.ToString(), requiredScopesResponse);
	}

	private static void AddInvalidRoleResponse(OpenApiOperation operation)
	{
		var invalidRoleResponse = new OpenApiResponse
		{
			Description = "Invalid role",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = new()
					{
						Reference = new()
						{
							Type = ReferenceType.Schema,
							Id = nameof(InvalidRoleErrorDto),
						},
					},
				},
			},
		};

		operation.Responses ??= [];
		_ = operation.Responses.TryAdd(PubNetHttpStatusCodes.Status461InvalidRole.ToString(), invalidRoleResponse);
	}

	private static IEnumerable<Attribute> GetOrderedAttributes(MethodInfo methodInfo, params Type[] attributeTypes)
	{
		return methodInfo
			.GetCustomAttributes(true)
			.Cast<Attribute>()
			.Concat(methodInfo.DeclaringType?.GetCustomAttributes(true).Cast<Attribute>() ?? [])
			.Where(a => attributeTypes.Any(t => t.IsAssignableFrom(a.GetType())));
	}

	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		document.EnsureAdded<InvalidRoleErrorDto>();
		document.EnsureAdded<MissingScopeErrorDto>();

		return Task.CompletedTask;
	}
}

file class RequiredScopesExtension(string[] scopes) : IOpenApiExtension
{
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		writer.WriteStartArray();

		foreach (var scope in scopes)
		{
			writer.WriteValue(scope);
		}

		writer.WriteEndArray();
	}
}

file class AnyOfRequiredScopesExtension(string[][] scopes) : IOpenApiExtension
{
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		writer.WriteStartArray();

		foreach (var group in scopes)
		{
			writer.WriteStartArray();

			foreach (var scope in group)
			{
				writer.WriteValue(scope);
			}

			writer.WriteEndArray();
		}

		writer.WriteEndArray();
	}
}

