using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DartLang.PubSpec.Serialization.Json;
using DartLang.PubSpec.Serialization.Json.Converters;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Converter;
using PubNet.API.DTO;

namespace PubNet.API.Extensions;

public static class JsonSerializerOptionsExtensions
{
	public static void ApplyCommonOptions(this JsonSerializerOptions options)
	{
		options.Converters.Add(new JsonDateTimeConverter());

		// add custom converters from DartLang.PubSpec
		options.Converters.Add(new SemVersionJsonConverter());
		options.Converters.Add(new SemVersionRangeJsonConverter());
		options.Converters.Add(new DependencyMapJsonConverter());
		options.Converters.Add(new PlatformsJsonConverter());

		options.TypeInfoResolver = JsonTypeInfoResolver.Combine(
			ApiSerializerContext.Default,
			DtoGenerationContext.Default,
			PubSpecJsonSerializerContext.Default
		);

		// apply defaults for JsonSerializerDefaults.Web
		options.PropertyNameCaseInsensitive = true;
		options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.NumberHandling = JsonNumberHandling.Strict; // so the schema only allows integers, not strings

		// most APIs will want to ignore null values
		options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
	}
}

#region Base Types

[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(byte[]))]

#endregion

#region Microsoft

[JsonSerializable(typeof(ProblemDetails))]

#endregion

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
public partial class ApiSerializerContext : JsonSerializerContext;
