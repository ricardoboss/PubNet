using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.OpenApi.Models;

namespace PubNet.API.OpenApi;

public static class OpenApiDocumentExtension
{
	public static void EnsureAdded<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
		T>(this OpenApiDocument document)
	{
		document.Components ??= new();
		document.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();

		if (document.Components.Schemas.ContainsKey(typeof(T).Name))
			return;

		document.Components.Schemas[typeof(T).Name] = GetOpenApiSchema(typeof(T));
	}

	private static OpenApiSchema GetOpenApiSchema(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
		Type type)
	{
		if (type == typeof(string))
			return new() { Type = "string" };

		if (type == typeof(int) || type == typeof(long))
			return new() { Type = "integer" };

		if (type == typeof(double) || type == typeof(decimal))
			return new() { Type = "number" };

		if (type == typeof(bool))
			return new() { Type = "boolean" };

		if (type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			var itemType = type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0];
			return new()
			{
				Type = "array",
				Items = GetOpenApiSchema(itemType)
			};
		}

		return new()
		{
			Type = "object",
			Properties = type.GetProperties().ToDictionary(
				p => TransformName(p.Name),
				p => GetOpenApiSchema(p.PropertyType)
			),
			Required = type.GetProperties().Where(p => Nullable.GetUnderlyingType(p.PropertyType) is null)
				.Select(p => p.Name).Select(TransformName).ToHashSet(),
		};
	}

	private static string TransformName(string name) => JsonNamingPolicy.CamelCase.ConvertName(name);
}
