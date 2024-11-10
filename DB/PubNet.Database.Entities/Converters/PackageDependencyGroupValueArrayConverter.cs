using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet.Packaging;

namespace PubNet.Database.Entities.Converters;

public class PackageDependencyGroupValueArrayConverter() : ValueConverter<PackageDependencyGroup[]?, string>(
	v => JsonSerializer.Serialize(v, Options),
	v => JsonSerializer.Deserialize<PackageDependencyGroup[]>(v, Options)
)
{
	private static readonly JsonSerializerOptions Options = new()
	{
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		WriteIndented = false,
		Converters =
		{
			new PackageDependencyGroupArrayConverter(),
			new PackageDependencyGroupConverter(),
			new PackageDependencyJsonConverter(),
			new VersionRangeJsonConverter(),
		},
	};
}
