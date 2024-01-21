using System.Text.Json;
using System.Text.Json.Serialization;

namespace PubNet.API.DTO;

public static class JsonSerializerOptionsExtensions
{
	public static void AddDtoSourceGenerators(this JsonSerializerOptions options)
	{
		options.PropertyNameCaseInsensitive = true;
		options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

		options.TypeInfoResolverChain.Add(DtoGenerationContext.Default);
	}
}
