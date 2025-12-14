using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PubNet.API.OpenApi;

public class IntSimplifyingTransformer : IOpenApiSchemaTransformer, IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
	{
		foreach (var (_, propSchemaInterface) in (IEnumerable<KeyValuePair<string, IOpenApiSchema>>?)schema.Properties ?? [])
		{
			SimplifyIntSchema(propSchemaInterface);
		}

		return Task.CompletedTask;
	}

	private static void SimplifyIntSchema(IOpenApiSchema maybeIntSchema)
	{
		if (maybeIntSchema is not OpenApiSchema { Format: "int32" or "int64" } intSchema)
			return;

		intSchema.Type = JsonSchemaType.Integer;
		intSchema.Format = null;
		intSchema.Pattern = null;
	}

	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		foreach (var param in operation.Parameters ?? [])
		{
			if (param.Schema != null)
				SimplifyIntSchema(param.Schema);
		}

		return Task.CompletedTask;
	}
}
