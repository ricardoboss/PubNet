using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using PubNet.API.DTO.Errors;
using PubNet.API.Extensions;

namespace PubNet.API.OpenApi;

public class ErrorsOperationTransformer : IOpenApiOperationTransformer
{
	private static async Task<OpenApiResponse> InternalServerErrorResponse(OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken) => new()
	{
		Description = "Internal Server Error",
		Content = new Dictionary<string, OpenApiMediaType>
		{
			["application/json"] = new()
			{
				Schema = await context.GetOrCreateSchemaAsync(typeof(InternalServerErrorDto),
					cancellationToken: cancellationToken),
			},
		},
	};

	private static async Task<OpenApiResponse> DefaultErrorResponse(OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken) => new()
	{
		Description = "Default Error",
		Content = new Dictionary<string, OpenApiMediaType>
		{
			["application/json"] = new()
			{
				Schema = await context.GetOrCreateSchemaAsync(typeof(StacktraceErrorDto),
					cancellationToken: cancellationToken),
			},
		},
	};

	private static async Task<OpenApiResponse> ValidationErrorsResponse(OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken) => new()
	{
		Description = "Bad Request",
		Content = new Dictionary<string, OpenApiMediaType>
		{
			["application/json"] = new()
			{
				Schema = await context.GetOrCreateSchemaAsync(typeof(ValidationErrorsDto),
					cancellationToken: cancellationToken),
			},
		},
	};

	public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		operation.Responses ??= [];

		if (operation.RequestBody != null)
			_ = operation.Responses.TryAdd("400", await ValidationErrorsResponse(context, cancellationToken));

		_ = operation.Responses.TryAdd("500", await InternalServerErrorResponse(context, cancellationToken));
		_ = operation.Responses.TryAdd("default", await DefaultErrorResponse(context, cancellationToken));
	}
}
