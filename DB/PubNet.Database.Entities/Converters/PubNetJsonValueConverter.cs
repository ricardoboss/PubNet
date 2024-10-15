using System.Text.Json;
using DartLang.PubSpec;
using DartLang.PubSpec.Serialization.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PubNet.Database.Entities.Converters;

public class PubSpecValueConverter() : ValueConverter<PubSpec, string>(
	v => JsonSerializer.Serialize(v, PubSpecJsonSerializerContext.Default.PubSpec),
	v => JsonSerializer.Deserialize(v, PubSpecJsonSerializerContext.Default.PubSpec)!
);
