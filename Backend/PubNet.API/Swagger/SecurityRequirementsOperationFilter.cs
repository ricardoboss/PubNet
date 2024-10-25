using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
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
		HandleAuthRequirementAndResponse(operation, context);

		HandleScopeRequirementAndResponse(operation, context);
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
		var scopes =
			GetOrderedAttributes(context.MethodInfo, typeof(RequireScopeAttribute), typeof(RequireAnyScopeAttribute))
				.SelectMany(a =>
					a switch
					{
						RequireScopeAttribute s => [s.Scope],
						RequireAnyScopeAttribute m => m.Scopes,
						_ => [],
					})
				.ToImmutableList();

		if (scopes.Count == 0)
			return;

		AddMissingRequiredScopeResponse(operation, context);
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
