using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using PubNet.API.DTO.Errors;

namespace PubNet.API.OpenApi;

public class ErrorsOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken)
	{
		if (operation.RequestBody != null)
		{
			operation.Responses["400"] = new OpenApiResponse
			{
				Description = "Bad Request",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					["application/json"] = new()
					{
						Schema = new OpenApiSchema
						{
							Reference = new OpenApiReference
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
					Schema = new OpenApiSchema
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.Schema,
							Id = nameof(InternalServerErrorDto),
						},
					},
				},
			},
		};

		_ = operation.Responses.TryAdd("500", internalServerErrorResponse);

		return Task.CompletedTask;
	}
}
