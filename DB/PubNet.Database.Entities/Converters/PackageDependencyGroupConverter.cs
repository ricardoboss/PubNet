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

		return reader.TokenType switch
		{
			JsonTokenType.Null => null,
			JsonTokenType.StartObject => ReadDependencyGroup(ref reader, options),
			_ => throw new JsonException("Expected start object or null"),
		};
	}

	private static PackageDependencyGroup ReadDependencyGroup(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException("Expected start object");

		NuGetFramework? targetFramework = null;
		var dependencies = new List<PackageDependency>();

		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			if (reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException("Expected property name");

			var propertyName = reader.GetString();
			if (propertyName is null)
				throw new JsonException("Missing property name");

			switch (propertyName)
			{
				case "targetFramework":
					if (!reader.Read() || reader.TokenType != JsonTokenType.String)
						throw new JsonException("Expected string");

					var targetFrameworkString = reader.GetString();
					if (targetFrameworkString is null)
						throw new JsonException("Missing target framework");

					targetFramework = NuGetFramework.Parse(targetFrameworkString);

					break;
				case "dependencies":
					if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
						throw new JsonException("Expected start array");

					ReadDependencies(ref reader, dependencies, options);

					break;
			}
		}

		if (targetFramework is null)
			throw new JsonException("Missing target framework");

		return new PackageDependencyGroup(targetFramework, dependencies);
	}

	private static void ReadDependencies(ref Utf8JsonReader reader, List<PackageDependency> dependencies, JsonSerializerOptions options)
	{
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			var dependency = JsonSerializer.Deserialize<PackageDependency>(ref reader, options);
			if (dependency is null)
				throw new JsonException("Failed to deserialize dependency");

			dependencies.Add(dependency);
		}
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
