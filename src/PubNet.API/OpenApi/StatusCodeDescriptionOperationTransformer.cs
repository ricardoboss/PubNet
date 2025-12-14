using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using PubNet.API.Controllers;

namespace PubNet.API.OpenApi;

public class StatusCodeDescriptionOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		foreach (var (code, response) in operation.Responses ?? [])
		{
			if (!string.IsNullOrEmpty(response.Description) ||
				response is not OpenApiResponse ||
				!int.TryParse(code, out var status)
			)
				continue;

			response.Description = PubNetStatusCodes.ToErrorMessage(status) ??
				throw new NotImplementedException("Missing error message for status code: " + code);

			// to ensure consistent behaviour
			_ = PubNetStatusCodes.ToErrorCode(status) ??
				throw new NotImplementedException("Missing error code for status code: " + code);
		}

		return Task.CompletedTask;
	}
}
