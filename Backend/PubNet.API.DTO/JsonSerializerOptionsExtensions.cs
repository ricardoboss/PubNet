using System.Text.Json;
using System.Text.Json.Serialization;
using DartLang.PubSpec.Serialization.Json;
using DartLang.PubSpec.Serialization.Json.Converters;

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

		// add custom converters from DartLang.PubSpec
		options.Converters.Add(new SemVersionJsonConverter());
		options.Converters.Add(new SemVersionRangeJsonConverter());
		options.Converters.Add(new DependencyMapJsonConverter());
		options.Converters.Add(new PlatformsJsonConverter());
		options.TypeInfoResolverChain.Add(PubSpecJsonSerializerContext.Default);

		options.TypeInfoResolverChain.Add(DtoGenerationContext.Default);
	}
}
