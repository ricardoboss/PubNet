using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using PubNet.API.DTO.Errors;

namespace PubNet.API.OpenApi;

public class ErrorsOperationTransformer : IOpenApiOperationTransformer, IOpenApiDocumentTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		if (operation.RequestBody != null)
		{
			operation.Responses["400"] = new()
			{
				Description = "Bad Request",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					["application/json"] = new()
					{
						Schema = new()
						{
							Reference = new()
							{
								Type = ReferenceType.Schema,
								Id = nameof(ValidationErrorsDto),
							},
						},
					},
				},
			};
		}

		// TODO: add InternalServerErrorDto schema to schemas collection if not exists
		var internalServerErrorResponse = new OpenApiResponse
		{
			Description = "Internal Server Error",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = new()
					{
						Reference = new()
						{
							Type = ReferenceType.Schema,
							Id = nameof(InternalServerErrorDto),
						},
					},
				},
			},
		};

		_ = operation.Responses.TryAdd("500", internalServerErrorResponse);

		var defaultErrorResponse = new OpenApiResponse
		{
			Description = "Default Error",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = new()
					{
						Reference = new()
						{
							Type = ReferenceType.Schema,
							Id = nameof(GenericErrorDto),
						},
					},
				},
			},
		};

		_ = operation.Responses.TryAdd("default", defaultErrorResponse);

		return Task.CompletedTask;
	}

	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		document.Components ??= new();
		document.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();
		document.Components.Schemas[nameof(InternalServerErrorDto)] = new()
		{
			Properties = new Dictionary<string, OpenApiSchema>
			{
				["error"] = new()
				{
					Type = "string",
					Nullable = false,
				},
				["message"] = new()
				{
					Type = "string",
					Nullable = false,
				},
				["stackTrace"] = new()
				{
					Type = "array",
					Items = new()
					{
						Type = "string",
						Nullable = false,
					},
				},
			},
		};

		return Task.CompletedTask;
	}
}
