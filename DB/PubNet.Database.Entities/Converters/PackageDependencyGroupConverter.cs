using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace PubNet.Database.Entities.Converters;

public class PackageDependencyGroupConverter : JsonConverter<PackageDependencyGroup>
{
	public override PackageDependencyGroup? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(PackageDependencyGroup))
			throw new NotSupportedException("Only PackageDependencyGroup can be deserialized");

		switch (reader.TokenType)
		{
			case JsonTokenType.Null:
				return null;
			case JsonTokenType.StartObject:
				return ReadDependencyGroup(ref reader);
			default:
				throw new JsonException("Expected start object or null");
		}
	}

	private static PackageDependencyGroup ReadDependencyGroup(ref Utf8JsonReader reader)
	{
		var targetFramework = reader.GetString();
		if (targetFramework is null)
			throw new JsonException("Missing target framework");

		var nugetFramework = NuGetFramework.Parse(targetFramework);
		var dependencies = new List<PackageDependency>();

		reader.Read();
		while (reader.TokenType != JsonTokenType.EndObject)
		{
			if (reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException("Expected property name");

			var propertyName = reader.GetString();
			if (propertyName is null)
				throw new JsonException("Missing property name");

			reader.Read();
			switch (propertyName)
			{
				case "dependencies":
					if (reader.TokenType != JsonTokenType.StartArray)
						throw new JsonException("Expected start array");

					reader.Read();
					while (reader.TokenType != JsonTokenType.EndArray)
					{
						var dependency = JsonSerializer.Deserialize<PackageDependency>(ref reader, options: null);
						if (dependency is null)
							throw new JsonException("Failed to deserialize dependency");

						dependencies.Add(dependency);
					}

					break;
				default:
					throw new JsonException($"Unknown property name: {propertyName}");
			}

			reader.Read();
		}

		return new PackageDependencyGroup(nugetFramework, dependencies);
	}

	public override void Write(Utf8JsonWriter writer, PackageDependencyGroup value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName("targetFramework");
		writer.WriteStringValue(value.TargetFramework.GetFrameworkString());

		if (!value.Packages.Any())
		{
			writer.WriteEndObject();

			return;
		}

		writer.WritePropertyName("dependencies");
		writer.WriteStartArray();

		foreach (var dependency in value.Packages)
		{
			JsonSerializer.Serialize(writer, dependency, options);
		}

		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
