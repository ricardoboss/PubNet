using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace PubNet.API.OpenApi;

public class SecurityRequirementsDocumentTransformer : IOpenApiDocumentTransformer
{
	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		document.Components ??= new();
		document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
		document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new()
		{
			Name = JwtBearerDefaults.AuthenticationScheme,
			Description = "The bearer token used to authenticate requests.",
			Type = SecuritySchemeType.Http,
			In = ParameterLocation.Header,
			Scheme = JwtBearerDefaults.AuthenticationScheme,
		};

		document.SecurityRequirements ??= [];
		document.SecurityRequirements.Add(
			new()
			{
				{
					new()
					{
						Reference = new()
						{
							Type = ReferenceType.SecurityScheme,
							Id = JwtBearerDefaults.AuthenticationScheme,
						},
					},
					[]
				},
			}
		);

// 		HandleAuthRequirementAndResponse(operation, context);
//
// 		HandleScopeRequirementAndResponse(operation, context);

		return Task.CompletedTask;
	}

// 	private static void HandleAuthRequirementAndResponse(OpenApiOperation operation, OpenApiOperationTransformerContext context)
// 	{
// 		var firstRelevantAttribute =
// 			GetOrderedAttributes(context.MethodInfo, typeof(AuthorizeAttribute), typeof(AllowAnonymousAttribute))
// 				.FirstOrDefault();
//
// 		// in case of null or 'Allow Anonymous' attribute, we don't add any security requirements
// 		if (firstRelevantAttribute is not AuthorizeAttribute authorizeAttribute)
// 			return;
//
// 		AddUnauthenticatedResponse(operation, context);
//
// 		AddInvalidRoleResponse(operation, context);
//
// 		var requiredScheme = authorizeAttribute.AuthenticationSchemes ?? JwtBearerDefaults.AuthenticationScheme;
//
// 		operation.Security.Add(
// 			new OpenApiSecurityRequirement
// 			{
// 				{
// 					new OpenApiSecurityScheme
// 					{
// 						Reference = new OpenApiReference
// 						{
// 							Type = ReferenceType.SecurityScheme,
// 							Id = requiredScheme,
// 						},
// 					},
// 					[]
// 				},
// 			}
// 		);
// 	}
//
// 	private static void AddUnauthenticatedResponse(OpenApiOperation operation, OpenApiOperationTransformerContext context)
// 	{
// 		var unauthenticatedResponse = new OpenApiResponse
// 		{
// 			Description = "Unauthenticated",
// 			Content = new Dictionary<string, OpenApiMediaType>
// 			{
// 				["application/json"] = new()
// 				{
// 					Schema = new OpenApiSchema
// 					{
// 						Reference = new OpenApiReference
// 						{
// 							Type = ReferenceType.Schema,
// 							Id = nameof(AuthErrorDto),
// 						},
// 					},
// 				},
// 			},
// 		};
//
// 		_ = operation.Responses.TryAdd("401", unauthenticatedResponse);
// 	}
//
// 	private static void HandleScopeRequirementAndResponse(OpenApiOperation operation, OpenApiOperationTransformerContext context)
// 	{
// 		var requiredScopes = GetOrderedAttributes(context.MethodInfo, typeof(RequireScopeAttribute))
// 			.Select(a => ((RequireScopeAttribute) a).Scope)
// 			.Select(s => s.Value)
// 			.ToArray();
// 		var anyOfRequiredScopes = GetOrderedAttributes(context.MethodInfo, typeof(RequireAnyScopeAttribute))
// 			.Select(a => ((RequireAnyScopeAttribute) a).Scopes)
// 			.Select(s => s.Select(ss => ss.Value).ToArray())
// 			.ToArray();
//
// 		if (requiredScopes.Length == 0 && anyOfRequiredScopes.Length == 0)
// 			return;
//
// 		AddMissingRequiredScopeResponse(operation, context);
//
// 		if (requiredScopes.Length > 0)
// 		{
// 			var requiredScopesExtension = new RequiredScopesExtension(requiredScopes);
//
// 			operation.Extensions.Add("x-required-scopes", requiredScopesExtension);
// 		}
//
// 		if (anyOfRequiredScopes.Length > 0)
// 		{
// 			var anyOfRequiredScopesExtension = new AnyOfRequiredScopesExtension(anyOfRequiredScopes);
//
// 			operation.Extensions.Add("x-any-of-required-scopes", anyOfRequiredScopesExtension);
// 		}
// 	}
//
// 	private static void AddMissingRequiredScopeResponse(OpenApiOperation operation, OpenApiOperationTransformerContext context)
// 	{
// 		var requiredScopesResponse = new OpenApiResponse
// 		{
// 			Description = "Missing required scope",
// 			Content = new Dictionary<string, OpenApiMediaType>
// 			{
// 				["application/json"] = new()
// 				{
// 					Schema = new OpenApiSchema
// 					{
// 						Reference = new OpenApiReference
// 						{
// 							Type = ReferenceType.Schema,
// 							Id = nameof(MissingScopeErrorDto),
// 						},
// 					},
// 				},
// 			},
// 		};
//
// 		_ = operation.Responses.TryAdd(PubNetHttpStatusCodes.Status460MissingScope.ToString(), requiredScopesResponse);
// 	}
//
// 	private static void AddInvalidRoleResponse(OpenApiOperation operation, OpenApiOperationTransformerContext context)
// 	{
// 		var invalidRoleResponse = new OpenApiResponse
// 		{
// 			Description = "Invalid role",
// 			Content = new Dictionary<string, OpenApiMediaType>
// 			{
// 				["application/json"] = new()
// 				{
// 					Schema = new OpenApiSchema
// 					{
// 						Reference = new OpenApiReference
// 						{
// 							Type = ReferenceType.Schema,
// 							Id = nameof(InvalidRoleErrorDto),
// 						},
// 					},
// 				},
// 			},
// 		};
//
// 		_ = operation.Responses.TryAdd(PubNetHttpStatusCodes.Status461InvalidRole.ToString(), invalidRoleResponse);
// 	}
//
// 	private static IEnumerable<Attribute> GetOrderedAttributes(MethodInfo methodInfo, params Type[] attributeTypes)
// 	{
// 		return methodInfo
// 			.GetCustomAttributes(true)
// 			.Cast<Attribute>()
// 			.Concat(methodInfo.DeclaringType?.GetCustomAttributes(true).Cast<Attribute>() ?? [])
// 			.Where(a => attributeTypes.Any(t => t.IsAssignableFrom(a.GetType())));
// 	}
}

// file class RequiredScopesExtension(string[] scopes) : IOpenApiExtension
// {
// 	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
// 	{
// 		writer.WriteStartArray();
//
// 		foreach (var scope in scopes)
// 		{
// 			writer.WriteValue(scope);
// 		}
//
// 		writer.WriteEndArray();
// 	}
// }
//
// file class AnyOfRequiredScopesExtension(string[][] scopes) : IOpenApiExtension
// {
// 	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
// 	{
// 		writer.WriteStartArray();
//
// 		foreach (var group in scopes)
// 		{
// 			writer.WriteStartArray();
//
// 			foreach (var scope in group)
// 			{
// 				writer.WriteValue(scope);
// 			}
//
// 			writer.WriteEndArray();
// 		}
//
// 		writer.WriteEndArray();
// 	}
// }
