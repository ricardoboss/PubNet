using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Packaging.Core;

namespace PubNet.Database.Entities.Converters;

internal class RepositoryMetadataJsonConverter : JsonConverter<RepositoryMetadata?>
{
	public override RepositoryMetadata? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(RepositoryMetadata))
			throw new NotSupportedException("Only RepositoryMetadata can be deserialized");

		return reader.TokenType switch
		{
			JsonTokenType.Null => null,
			JsonTokenType.StartObject => ReadMetadata(ref reader),
			_ => throw new JsonException("Expected start object or null"),
		};
	}

	private static RepositoryMetadata ReadMetadata(ref Utf8JsonReader reader)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException("Expected start object");

		var type = "";
		var url = "";
		var branch = "";
		var commit = "";

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
				case "type":
					if (reader.TokenType != JsonTokenType.String)
						throw new JsonException("Expected string");

					type = reader.GetString();
					break;
				case "url":
					if (reader.TokenType != JsonTokenType.String)
						throw new JsonException("Expected string");

					url = reader.GetString();
					break;
				case "branch":
					if (reader.TokenType != JsonTokenType.String)
						throw new JsonException("Expected string");

					branch = reader.GetString();
					break;
				case "commit":
					if (reader.TokenType != JsonTokenType.String)
						throw new JsonException("Expected string");

					commit = reader.GetString();
					break;
				default:
					throw new JsonException($"Unexpected property name: {propertyName}");
			}
		}

		return new RepositoryMetadata(type, url, branch, commit);
	}

	public override void Write(Utf8JsonWriter writer, RepositoryMetadata? value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();

			return;
		}

		writer.WriteStartObject();

		if (!string.IsNullOrWhiteSpace(value.Type))
		{
			writer.WritePropertyName("type");
			writer.WriteStringValue(value.Type);
		}

		if (!string.IsNullOrWhiteSpace(value.Url))
		{
			writer.WritePropertyName("url");
			writer.WriteStringValue(value.Url);
		}

		if (!string.IsNullOrWhiteSpace(value.Branch))
		{
			writer.WritePropertyName("branch");
			writer.WriteStringValue(value.Branch);
		}

		if (!string.IsNullOrWhiteSpace(value.Commit))
		{
			writer.WritePropertyName("commit");
			writer.WriteStringValue(value.Commit);
		}

		writer.WriteEndObject();
	}
}
