using System.Text.Json;
using System.Text.Json.Serialization;

namespace PubNet.API.DTO;

public static class JsonSerializerOptionsExtensions
{
	public static void AddDtoSourceGenerators(this JsonSerializerOptions options)
	{
		// apply defaults for JsonSerializerDefaults.Web
		options.PropertyNameCaseInsensitive = true;
		options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.NumberHandling = JsonNumberHandling.AllowReadingFromString;

		// most APIs will want to ignore null values
		options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

		options.TypeInfoResolverChain.Add(DtoGenerationContext.Default);
	}
}
