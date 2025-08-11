using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.OpenApi;

namespace PubNet.API.OpenApi;

public static class OpenApiDocumentExtension
{
	public static void EnsureAdded<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
		T>(this OpenApiDocument document)
	{
		document.Components ??= new();
		document.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();

		if (document.Components.Schemas.ContainsKey(typeof(T).Name))
			return;

		document.Components.Schemas[typeof(T).Name] = GetOpenApiSchema(typeof(T));
	}

	private static OpenApiSchema GetOpenApiSchema(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
		Type type)
	{
		var primitive = type.MapTypeToOpenApiPrimitiveType();
		if (primitive is { Type: not JsonSchemaType.String and not JsonSchemaType.Object and not JsonSchemaType.Array })
			return primitive;

		if (type == typeof(string))
			return new() { Type = JsonSchemaType.String };

		if (type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			var itemType = type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0];
			return new()
			{
				Type = JsonSchemaType.Array,
				Items = GetOpenApiSchema(itemType),
			};
		}

		return new()
		{
			Type = JsonSchemaType.Object,
			Properties = type.GetProperties().ToDictionary(
				p => TransformName(p.Name),
				IOpenApiSchema (p) => GetOpenApiSchema(p.PropertyType)
			),
			Required = type.GetProperties().Where(p => Nullable.GetUnderlyingType(p.PropertyType) is null)
				.Select(p => p.Name).Select(TransformName).ToHashSet(),
		};
	}

	private static string TransformName(string name) => JsonNamingPolicy.CamelCase.ConvertName(name);
}
