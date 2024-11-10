using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet.Packaging.Core;

namespace PubNet.Database.Entities.Converters;

public class RepositoryMetadataValueConverter() : ValueConverter<RepositoryMetadata?, string>(
	v => JsonSerializer.Serialize(v, Options),
	v => JsonSerializer.Deserialize<RepositoryMetadata>(v, Options)
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
			new RepositoryMetadataJsonConverter(),
		},
	};
}
