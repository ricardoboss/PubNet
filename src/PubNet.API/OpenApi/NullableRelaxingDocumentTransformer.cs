using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PubNet.API.OpenApi;

public sealed class NullableRelaxingDocumentTransformer : IOpenApiDocumentTransformer
{
	public Task TransformAsync(
		OpenApiDocument document,
		OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		foreach (var schema in document.Components?.Schemas?.Values ?? [])
		{
			if (schema is OpenApiSchema s)
				Rewrite(s);
		}

		foreach (var path in document.Paths.Values)
		{
			foreach (var op in path.Operations?.Values ?? Enumerable.Empty<OpenApiOperation>())
			{
				if (op.RequestBody != null)
					RewriteContent(op.RequestBody.Content);

				foreach (var response in op.Responses?.Values ?? Enumerable.Empty<IOpenApiResponse>())
					RewriteContent(response.Content);
			}
		}

		return Task.CompletedTask;
	}

	private static void RewriteContent(IDictionary<string, OpenApiMediaType>? content)
	{
		if (content == null)
			return;

		foreach (var mediaType in content.Values)
		{
			if (mediaType.Schema is OpenApiSchema s)
				Rewrite(s);
		}
	}

	private static void Rewrite(OpenApiSchema schema)
	{
		// detect wrapper: oneOf = [null-type, ref]
		if (schema.OneOf is { Count: 2 } oneOf &&
			oneOf.Any(s => s.Type == JsonSchemaType.Null))
		{
			var nonNull = oneOf.First(s => s.Type != JsonSchemaType.Null);

			schema.OneOf = null;
			schema.AnyOf = new List<IOpenApiSchema>
			{
				nonNull,
				new OpenApiSchema { Type = JsonSchemaType.Null },
			};
		}

		// recurse into properties / items
		if (schema.Properties != null)
		{
			foreach (var p in schema.Properties.Values)
				if (p is OpenApiSchema s)
					Rewrite(s);
		}

		if (schema.Items is OpenApiSchema itemsSchema)
			Rewrite(itemsSchema);
	}
}
