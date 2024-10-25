using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PubNet.API.Swagger;

internal static class OperationFilterHelper
{
    public static OpenApiSchema GetOrAddDtoSchema<TDto>(this OperationFilterContext context)
    {
        var dtoType = typeof(TDto);
        if (!context.SchemaRepository.TryLookupByType(dtoType, out var dtoSchema))
        {
            dtoSchema = context.SchemaGenerator.GenerateSchema(dtoType, context.SchemaRepository);
        }

        return dtoSchema;
    }
}
