using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace PubNet.Database.Entities.Converters;

public class PackageDependencyJsonConverter : JsonConverter<PackageDependency>
{
	public override PackageDependency? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(PackageDependency))
			throw new NotSupportedException("Only PackageDependency can be deserialized");

		return reader.TokenType switch
		{
			JsonTokenType.Null => null,
			JsonTokenType.StartObject => ReadDependency(ref reader, options),
			_ => throw new JsonException("Expected start object or null"),
		};
	}

	private static PackageDependency ReadDependency(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException("Expected start object");

		string? id = null;
		VersionRange? versionRange = null;
		List<string> include = [];
		List<string> exclude = [];

		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			if (reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException("Expected property name");

			var propertyName = reader.GetString();
			if (propertyName is null)
				throw new JsonException("Missing property name");

			if (!reader.Read())
				throw new JsonException("Expected value");

			switch (propertyName)
			{
				case "id":
					if (reader.TokenType != JsonTokenType.String)
						throw new JsonException("Expected string");

					id = reader.GetString();
					break;
				case "version":
					versionRange = JsonSerializer.Deserialize<VersionRange>(ref reader, options);
					break;
				case "include":
					if (reader.TokenType != JsonTokenType.StartArray)
						throw new JsonException("Expected start array");

					ReadStringArray(ref reader, include, options);
					break;
				case "exclude":
					if (reader.TokenType != JsonTokenType.StartArray)
						throw new JsonException("Expected start array");

					ReadStringArray(ref reader, exclude, options);
					break;
				default:
					throw new JsonException($"Unexpected property name: {propertyName}");
			}
		}

		if (id is null)
			throw new JsonException("Missing id");

		if (versionRange is null)
			throw new JsonException("Missing version range");

		return new PackageDependency(id, versionRange, include, exclude);
	}

	private static void ReadStringArray(ref Utf8JsonReader reader, List<string> includes, JsonSerializerOptions options)
	{
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			if (reader.TokenType != JsonTokenType.String)
				throw new JsonException("Expected string");

			includes.Add(reader.GetString()!);
		}
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
			writer.WriteStartArray();

			foreach (var exclude in value.Exclude)
			{
				writer.WriteStringValue(exclude);
			}

			writer.WriteEndArray();
		}

		if (value.Include.Any())
		{
			writer.WritePropertyName("include");
			writer.WriteStartArray();

			foreach (var include in value.Include)
			{
				writer.WriteStringValue(include);
			}

			writer.WriteEndArray();
		}

		writer.WriteEndObject();
	}
}
