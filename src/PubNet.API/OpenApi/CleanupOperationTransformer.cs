using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PubNet.API.OpenApi;

public class CleanupOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		if (operation.RequestBody?.Content?.ContainsKey("text/json") ?? false)
			operation.RequestBody.Content.Remove("text/json");

		if (operation.RequestBody?.Content?.ContainsKey("application/*+json") ?? false)
			operation.RequestBody.Content.Remove("application/*+json");

		foreach (var response in operation.Responses?.Values ?? Enumerable.Empty<IOpenApiResponse>())
		{
			response.Content?.Remove("text/plain");
			response.Content?.Remove("text/json");
		}

		return Task.CompletedTask;
	}
}
