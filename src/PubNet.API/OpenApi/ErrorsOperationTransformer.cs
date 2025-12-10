using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using PubNet.API.DTO.Errors;
using PubNet.API.Extensions;

namespace PubNet.API.OpenApi;

public class ErrorsOperationTransformer : IOpenApiOperationTransformer, IOpenApiDocumentTransformer
{
	private static readonly OpenApiResponse InternalServerErrorResponse = new()
	{
		Description = "Internal Server Error",
		Content = new Dictionary<string, OpenApiMediaType>
		{
			["application/json"] = new()
			{
				Schema = new OpenApiSchemaReference(nameof(InternalServerErrorDto)),
			},
		},
	};

	private static readonly OpenApiResponse DefaultErrorResponse = new()
	{
		Description = "Default Error",
		Content = new Dictionary<string, OpenApiMediaType>
		{
			["application/json"] = new()
			{
				Schema = new OpenApiSchemaReference(nameof(GenericErrorDto)),
			},
		},
	};

	private static readonly OpenApiResponse ValidationErrorsResponse = new()
	{
		Description = "Bad Request",
		Content = new Dictionary<string, OpenApiMediaType>
		{
			["application/json"] = new()
			{
				Schema = new OpenApiSchemaReference(nameof(ValidationErrorsDto)),
			},
		},
	};

	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		operation.Responses ??= [];

		if (operation.RequestBody != null)
			_ = operation.Responses.TryAdd("400", ValidationErrorsResponse);

		_ = operation.Responses.TryAdd("500", InternalServerErrorResponse);
		_ = operation.Responses.TryAdd("default", DefaultErrorResponse);

		return Task.CompletedTask;
	}

	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		document.EnsureAdded<InternalServerErrorDto>();
		document.EnsureAdded<ValidationErrorsDto>();
		document.EnsureAdded<GenericErrorDto>();

		return Task.CompletedTask;
	}
}
