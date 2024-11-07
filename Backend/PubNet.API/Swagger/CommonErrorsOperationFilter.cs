using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using PubNet.API.DTO.Errors;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PubNet.API.Swagger;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by Swashbuckle.")]
internal sealed class CommonErrorsOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
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
						Schema = context.GetOrAddDtoSchema<ValidationErrorsDto>(),
					},
				},
			};

			// operation.Responses["422"] = new OpenApiResponse
			// {
			//     Description = "Unprocessable Entity",
			//     Content = new Dictionary<string, OpenApiMediaType>
			//     {
			//         ["application/json"] = new()
			//         {
			//             Schema = context.GetOrAddDtoSchema<UnprocessableEntityErrorDto>(),
			//         },
			//     },
			// };
		}

		// var tooManyRequestsResponse = new OpenApiResponse
		// {
		//     Description = "Too Many Requests",
		//     Content = new Dictionary<string, OpenApiMediaType>
		//     {
		//         ["application/json"] = new()
		//         {
		//             Schema = context.GetOrAddDtoSchema<TooManyRequestsErrorDto>(),
		//         },
		//     },
		// };

		var internalServerErrorResponse = new OpenApiResponse
		{
			Description = "Internal Server Error",
			Content = new Dictionary<string, OpenApiMediaType>
			{
				["application/json"] = new()
				{
					Schema = context.GetOrAddDtoSchema<InternalServerErrorDto>(),
				},
			},
		};

		// _ = operation.Responses.TryAdd("429", tooManyRequestsResponse);
		_ = operation.Responses.TryAdd("500", internalServerErrorResponse);
	}
}
