using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using PubNet.API.Attributes;
using PubNet.API.DTO.Errors;
using PubNet.Auth;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PubNet.API.Swagger;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by Swashbuckle.")]
internal sealed class SecurityRequirementsOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		RemoveContentTypesOtherThanJson(operation);

		HandleAuthRequirementAndResponse(operation, context);

		HandleScopeRequirementAndResponse(operation, context);
	}

	private static void RemoveContentTypesOtherThanJson(OpenApiOperation operation)
	{
		if (operation.RequestBody?.Content?.ContainsKey("text/json") ?? false)
			operation.RequestBody.Content.Remove("text/json");

		if (operation.RequestBody?.Content?.ContainsKey("application/*+json") ?? false)
			operation.RequestBody.Content.Remove("application/*+json");

		foreach (var response in operation.Responses.Values)
		{
			response.Content.Remove("text/plain");
			response.Content.Remove("text/json");
		}
	}

	private static void HandleAuthRequirementAndResponse(OpenApiOperation operation, OperationFilterContext context)
	{
		var firstRelevantAttribute =
			GetOrderedAttributes(context.MethodInfo, typeof(AuthorizeAttribute), typeof(AllowAnonymousAttribute))
				.FirstOrDefault();

		// in case of null or 'Allow Anonymous' attribute, we don't add any security requirements
		if (firstRelevantAttribute is not AuthorizeAttribute authorizeAttribute)
			return;

		AddUnauthenticatedResponse(operation, context);

		AddInvalidRoleResponse(operation, context);

		var requiredScheme = authorizeAttribute.AuthenticationSchemes ?? JwtBearerDefaults.AuthenticationScheme;

		operation.Security.Add(
			new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
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

	private static void AddUnauthenticatedResponse(OpenApiOperation operation, OperationFilterContext context)
	{
		var unauthenticatedResponse = new OpenApiResponse
		{
			Description = "Unauthenticated",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = context.GetOrAddDtoSchema<AuthErrorDto>(),
				},
			},
		};

		_ = operation.Responses.TryAdd("401", unauthenticatedResponse);
	}

	private static void HandleScopeRequirementAndResponse(OpenApiOperation operation, OperationFilterContext context)
	{
		var requiredScopes = GetOrderedAttributes(context.MethodInfo, typeof(RequireScopeAttribute))
			.Select(a => ((RequireScopeAttribute) a).Scope)
			.Select(s => s.Value)
			.ToArray();
		var anyOfRequiredScopes = GetOrderedAttributes(context.MethodInfo, typeof(RequireAnyScopeAttribute))
			.Select(a => ((RequireAnyScopeAttribute) a).Scopes)
			.Select(s => s.Select(ss => ss.Value).ToArray())
			.ToArray();

		if (requiredScopes.Length == 0 && anyOfRequiredScopes.Length == 0)
			return;

		AddMissingRequiredScopeResponse(operation, context);

		if (requiredScopes.Length > 0)
		{
			var requiredScopesExtension = new RequiredScopesExtension(requiredScopes);

			operation.Extensions.Add("x-required-scopes", requiredScopesExtension);
		}

		if (anyOfRequiredScopes.Length > 0)
		{
			var anyOfRequiredScopesExtension = new AnyOfRequiredScopesExtension(anyOfRequiredScopes);

			operation.Extensions.Add("x-any-of-required-scopes", anyOfRequiredScopesExtension);
		}
	}

	private static void AddMissingRequiredScopeResponse(OpenApiOperation operation, OperationFilterContext context)
	{
		var requiredScopesResponse = new OpenApiResponse
		{
			Description = "Missing required scope",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = context.GetOrAddDtoSchema<MissingScopeErrorDto>(),
				},
			},
		};

		_ = operation.Responses.TryAdd(PubNetHttpStatusCodes.Status460MissingScope.ToString(), requiredScopesResponse);
	}

	private static void AddInvalidRoleResponse(OpenApiOperation operation, OperationFilterContext context)
	{
		var invalidRoleResponse = new OpenApiResponse
		{
			Description = "Invalid role",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = context.GetOrAddDtoSchema<InvalidRoleErrorDto>(),
				},
			},
		};

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
