using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Versioning;

namespace PubNet.Database.Entities.Converters;

public class VersionRangeJsonConverter : JsonConverter<VersionRange>
{
	public override VersionRange? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(VersionRange))
			throw new NotSupportedException("Only VersionRange can be deserialized");

		switch (reader.TokenType)
		{
			case JsonTokenType.Null:
				return null;
			case JsonTokenType.String:
				return ReadVersionRange(reader);
			default:
				throw new JsonException("Expected string or null");
		}
	}

	private static VersionRange ReadVersionRange(Utf8JsonReader reader)
	{
		var range = reader.GetString();
		if (range is null)
			throw new JsonException("Expected string");

		if (!VersionRange.TryParse(range, out var versionRange))
			throw new JsonException("Invalid version range");

		return versionRange;
	}

	public override void Write(Utf8JsonWriter writer, VersionRange value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToNormalizedString());
	}
}
