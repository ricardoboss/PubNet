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

		switch (reader.TokenType)
		{
			case JsonTokenType.Null:
				return null;
			case JsonTokenType.StartObject:
				return ReadMetadata(ref reader);
			default:
				throw new JsonException("Expected start object or null");
		}
	}

	private static RepositoryMetadata ReadMetadata(ref Utf8JsonReader reader)
	{
		throw new NotImplementedException();
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
