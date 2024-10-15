using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PubNet.API.Converter;

public class JsonDateTimeConverter : JsonConverter<DateTimeOffset>
{
	public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		Debug.Assert(typeToConvert == typeof(DateTimeOffset));

		return DateTimeOffset.Parse(reader.GetString() ?? throw new InvalidOperationException());
	}

	public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffZ"));
	}
}
