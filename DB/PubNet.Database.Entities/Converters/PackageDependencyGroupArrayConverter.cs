using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Packaging;

namespace PubNet.Database.Entities.Converters;

public class PackageDependencyGroupArrayConverter : JsonConverter<PackageDependencyGroup[]>
{
	public override PackageDependencyGroup[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(PackageDependencyGroup[]))
			throw new NotSupportedException("Only PackageDependencyGroup[] can be deserialized");

		if (reader.TokenType == JsonTokenType.Null)
			return null;

		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected start array");

		var dependencyGroups = new List<PackageDependencyGroup>();

		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			var group = JsonSerializer.Deserialize<PackageDependencyGroup>(ref reader, options);
			if (group is null)
				throw new JsonException("Expected PackageDependencyGroup");

			dependencyGroups.Add(group);
		}

		return dependencyGroups.ToArray();
	}

	public override void Write(Utf8JsonWriter writer, PackageDependencyGroup[] value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();

		foreach (var dependencyGroup in value)
		{
			JsonSerializer.Serialize(writer, dependencyGroup, options);
		}

		writer.WriteEndArray();
	}
}
