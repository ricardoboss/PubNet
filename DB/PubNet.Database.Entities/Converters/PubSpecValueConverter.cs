using System.Text.Json;
using DartLang.PubSpec;
using DartLang.PubSpec.Serialization.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PubNet.Database.Entities.Converters;

public class PubSpecValueConverter() : ValueConverter<PubSpec?, string?>(
	v => v == null ? null : JsonSerializer.Serialize(v, PubSpecJsonSerializerContext.Default.PubSpec),
	v => v == null ? null : JsonSerializer.Deserialize(v, PubSpecJsonSerializerContext.Default.PubSpec)
);
