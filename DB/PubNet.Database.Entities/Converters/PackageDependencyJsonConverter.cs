using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Packaging.Core;

namespace PubNet.Database.Entities.Converters;

public class PackageDependencyJsonConverter : JsonConverter<PackageDependency>
{
	public override PackageDependency? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(PackageDependency))
			throw new NotSupportedException("Only PackageDependency can be deserialized");

		switch (reader.TokenType)
		{
			case JsonTokenType.Null:
				return null;
			case JsonTokenType.StartObject:
				return ReadDependency(ref reader);
			default:
				throw new JsonException("Expected start object or null");
		}
	}

	private static PackageDependency ReadDependency(ref Utf8JsonReader reader)
	{
		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, PackageDependency value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName("id");
		writer.WriteStringValue(value.Id);

		writer.WritePropertyName("version");
		JsonSerializer.Serialize(writer, value.VersionRange, options);

		if (value.Exclude.Any())
		{
			writer.WritePropertyName("exclude");
			writer.WriteStringValue(value.Id);
		}

		if (value.Include.Any())
		{
			writer.WritePropertyName("include");
			writer.WriteStringValue(value.Id);
		}

		writer.WriteEndObject();
	}
}
